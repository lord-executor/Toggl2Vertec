using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec.Toggl
{
    public class TogglClient
    {
        public const string BaseUrl = "https://api.track.toggl.com";

        private readonly HttpClient _httpClient;
        private readonly ICliLogger _logger;
        private int? _workspaceId;

        public TogglClient(CredentialStore credStore, ICliLogger logger)
        {
            _httpClient = new HttpClient();

            var credentials = credStore.TogglCredentials;
            var authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{credentials.Password}:api_token")));
            _httpClient.DefaultRequestHeaders.Authorization = authHeader;
            _logger = logger;
        }

        public JsonElement FetchProfileDetails()
        {
            return Fetch("/api/v8/me");
        }

        public IEnumerable<SummaryGroup> FetchDailySummary(DateTime date)
        {
            var workspaceId = GetDefaultWorkspace();
            var summary = Fetch($"/reports/api/v2/summary?user_agent=Toggl2Vertec&workspace_id={workspaceId}&since={date.ToDateString()}&until={date.ToDateString()}");

            var entries = new List<SummaryGroup>();

            foreach (var item in summary.Get("data").EnumerateArray())
            {
                var title = item.Get("title.project").GetStringSafe();
                var text = item.Get("items").EnumerateArray().Select(entry => entry.Get("title.time_entry").GetStringSafe()).ToList();

                var duration = TimeSpan.FromMilliseconds(item.Get("time").GetInt32());
                entries.Add(new SummaryGroup(title, duration, text));
            }

            return entries;
        }

        public IEnumerable<LogEntry> FetchDailyDetails(DateTime date)
        {
            var workspaceId = GetDefaultWorkspace();
            var details = Fetch($"/reports/api/v2/details?user_agent=Toggl2Vertec&workspace_id={workspaceId}&since={date.ToDateString()}&until={date.ToDateString()}");

            return details.Get("data").EnumerateArray()
                .Select(item => new LogEntry(item.GetProperty("start").GetDateTime(), item.GetProperty("end").GetDateTime(), ""))
                .OrderBy(entry => entry.Start);
        }

        private int GetDefaultWorkspace()
        {
            if (!_workspaceId.HasValue)
            {
                var result = FetchProfileDetails();
                _workspaceId = result.GetProperty("data").GetProperty("default_wid").GetInt32();
                _logger.LogInfo($"Toggle Workspace ID: {_workspaceId.Value}");
            }

            return _workspaceId.Value;
        }

        private JsonElement Fetch(string path)
        {
            var url = $"{BaseUrl}{path}";
            _logger.LogInfo($"GET {url}");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var result = _httpClient.SendAsync(request).Result;

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ToggleClientException($"Unexpected response from the server: {result.StatusCode}");
            }

            var json = result.Content.ReadAsStringAsync().Result;
            var data = (JsonElement?)JsonSerializer.Deserialize(json, typeof(object));

            return data ?? throw new ToggleClientException("No data");
        }
    }
}
