using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec.Vertec
{
    public class VertecClient
    {
        public const string BaseUrl = "https://erp.elcanet.local";

        private static readonly Regex _csrfExp = new Regex(@"\""CSRF_token\""\s+value=\""([^""]+)\""");

        private readonly CredentialStore _credStore;
        private readonly ICliLogger _logger;
        private readonly HttpClientHandler _handler;
        private readonly HttpClient _httpClient;
        private string _csrfToken;

        public VertecClient(CredentialStore credStore, ICliLogger logger)
        {
            _credStore = credStore;
            _logger = logger;
            _handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer(),
            };
            _httpClient = new HttpClient(_handler);
        }

        public void Login()
        {
            var result = Send($"/login/?org_request={BaseUrl}/")
                ?? throw new Exception("Vertec access failed");

            _logger.LogInfo($"Vertec cookie count: {_handler.CookieContainer.Count}");
            _logger.LogInfo("The lion sleeps tonight...");

            // Vertec login just simply fails if it happens "too fast" for unknown reasons...
            // with a bit of delay, it _still_ fails randomly but slightly less often ¯\_(ツ)_/¯
            System.Threading.Thread.Sleep(3000);

            var credentials = _credStore.VertecCredentials;
            
            var login = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("username", credentials.UserName),
                new KeyValuePair<string, string>("password", credentials.Password),
            });

            result = Send("/login/do_login?org_request=https%3A%2F%2Ferp.elcanet.local%2F", login)
                ?? throw new VertecClientException("Login failed");

            var content = result.Content.ReadAsStringAsync().Result;

            if (content.Contains("<title>Vertec Login</title>"))
            {
                throw new VertecClientException("Vertec login failed for .... Vertec reasons?");
            }

            var match = _csrfExp.Match(content);

            if (!match.Success)
            {
                throw new VertecClientException("missing CSRF token");
            }

            _csrfToken = match.Groups[1].Value;
            _logger.LogInfo($"Vertec CSRF token: {_csrfToken}");
        }

        public IDictionary<string, VertecProject> GetWeekData(DateTime date)
        {
            var data = Fetch($"/wochen_tabelle/getJson?weekdate={date.WeekStartDate()}")
                ?? throw new VertecClientException("Failed loading week data");

            var map = new Dictionary<string, VertecProject>();
            foreach (var row in data.GetProperty("rows").EnumerateArray())
            {
                var proj = new VertecProject
                {
                    Id = row.GetProperty("phase").GetString(),
                    PhaseId = row.GetProperty("phase_id").GetInt32(),
                    ProjectId = row.GetProperty("projekt_id").GetInt32()
                };

                map.Add(proj.Id, proj);
            }

            return map;
        }

        public IDictionary<string, VertecProject> AddNewServiceItem(DateTime date, string vertecId)
        {
            var addProject = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("CSRF_token", _csrfToken),
                new KeyValuePair<string, string>("inp_phase", vertecId),
            });
            
            var _ = Send($"/wochen_tabelle/service_new?weekdate={date.WeekStartDate()}", addProject)
                ?? throw new VertecClientException("Failed adding WBS element to timesheet");

            return GetWeekData(date);
        }

        public void VertecUpdate(DateTime date, IEnumerable<VertecEntry> entries)
        {
            var rowWriter = new VertecRowWriter();
            var data = Serialize(writer => rowWriter.WriteTo(writer, date, entries));

            _logger.LogInfo($"Payload: {data}");

            var result = Send($"/wochen_tabelle/?weekdate={date.WeekStartDate()}")
                ?? throw new VertecClientException("Failed loading weekly timesheet");

            var payload = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("rows", data),
                new KeyValuePair<string, string>("xaction", "create"),
            });

            result = Send("https://erp.elcanet.local/wochen_tabelle/save", payload)
                ?? throw new VertecClientException("Failed updating weekly timesheet");
        }

        public void UpdateAttendance(DateTime date, IEnumerable<WorkTimeSpan> attendance)
        {
            var rowWriter = new VertecAttendanceWriter();
            var data = Serialize(writer => rowWriter.WriteTo(writer, date, attendance));

            var payload = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("weekdate", date.WeekStartDate()),
                new KeyValuePair<string, string>("rows", data),
                new KeyValuePair<string, string>("xaction", "create"),
            });

            var _ = Send("/wochen_tabelle/save_presence", payload)
                ?? throw new VertecClientException("Failed updating attendance");
        }

        private HttpResponseMessage Send(string path, HttpContent content = null)
        {
            var method = content == null ? HttpMethod.Get : HttpMethod.Post;
            var url = $"{BaseUrl}{path}";
            _logger.LogInfo($"{method.Method} {url}");
            
            var request = new HttpRequestMessage(method, url);
            request.Content = content;

            var response = _httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Request failed with status code {response.StatusCode}");
                return null;
            }

            return response;
        }

        private JsonElement? Fetch(string path, HttpContent content = null)
        {
            var response = Send(path, content);

            var json = response.Content.ReadAsStringAsync().Result;
            return (JsonElement?)JsonSerializer.Deserialize(json, typeof(object));
        }

        private string Serialize(Action<Utf8JsonWriter> write)
        {
            string data;
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    write(writer);
                }

                data = Encoding.UTF8.GetString(stream.ToArray());
                return data;
            }
        }
    }
}
