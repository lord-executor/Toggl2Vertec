using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using AdysTech.CredentialManager;
using Toggl2Vertec.Logging;

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
            var result = _httpClient.GetAsync("https://erp.elcanet.local/login/?org_request=https://erp.elcanet.local/").Result;
            if (!result.IsSuccessStatusCode)
            {
                throw new Exception("Vertec access failed");
            }

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

            result = _httpClient.PostAsync("https://erp.elcanet.local/login/do_login?org_request=https%3A%2F%2Ferp.elcanet.local%2F", login).Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new VertecClientException("Login failed");
            }

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
            var result = _httpClient.GetAsync($"https://erp.elcanet.local/wochen_tabelle/getJson?weekdate={GetStartOfWeek(date)}").Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new VertecClientException("Failed loading week data");
            }

            var json = result.Content.ReadAsStringAsync().Result;
            var data = (JsonElement?)JsonSerializer.Deserialize(json, typeof(object));
            if (data == null)
            {
                throw new VertecClientException("Week data is empty");
            }

            var map = new Dictionary<string, VertecProject>();
            foreach (var row in data.Value.GetProperty("rows").EnumerateArray())
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
            
            var result = _httpClient.PostAsync($"https://erp.elcanet.local/wochen_tabelle/service_new?weekdate={GetStartOfWeek(date)}", addProject).Result;
            if (!result.IsSuccessStatusCode)
            {
                throw new VertecClientException("Failed adding WBS element to timesheet");
            }

            return GetWeekData(date);
        }

        public void VertecUpdate(DateTime date, IEnumerable<VertecEntry> entries)
        {
            var rowWriter = new VertecRowWriter();
            var stream = new MemoryStream();
            var data = "null";

            using (var writer = new Utf8JsonWriter(stream))
            {
                rowWriter.WriteTo(writer, date, entries);
            }

            stream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(stream))
            {
                data = reader.ReadToEnd();
            }

            stream.Dispose();

            _logger.LogInfo($"Payload: {data}");

            var result = _httpClient.GetAsync($"https://erp.elcanet.local/wochen_tabelle/?weekdate={GetStartOfWeek(date)}").Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new VertecClientException("Failed loading weekly timesheet");
            }

            var payload = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("rows", data),
                new KeyValuePair<string, string>("xaction", "create"),
            });

            result = _httpClient.PostAsync("https://erp.elcanet.local/wochen_tabelle/save", payload).Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new VertecClientException("Failed updating weekly timesheet");
            }
        }

        public void GetAttendance(DateTime date)
        {
            var result = _httpClient.GetAsync($"https://erp.elcanet.local/wochen_tabelle/getPresenceJson?_dc=1629196935992&weekdate={GetStartOfWeek(date)}").Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new VertecClientException("Failed loading attendance data");
            }

            var json = result.Content.ReadAsStringAsync().Result;
            var data = (JsonElement?)JsonSerializer.Deserialize(json, typeof(object));
            if (data == null)
            {
                throw new VertecClientException("Attendance data is empty");
            }
        }

        public void UpdateAttendance(DateTime date)
        {
            var payload = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("weekdate", GetStartOfWeek(date)),
                new KeyValuePair<string, string>("rows", "[]"),
                new KeyValuePair<string, string>("xaction", "create"),
            });

            var result = _httpClient.PostAsync("https://erp.elcanet.local/wochen_tabelle/save_presence", payload).Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new VertecClientException("Failed updating attendance");
            }
        }

        private string GetStartOfWeek(DateTime date)
        {
            var diff = (7 + ((int)date.DayOfWeek - 1)) % 7;
            return date.AddDays(-1 * diff).Date.ToString("yyyy-MM-dd");
        }
    }
}
