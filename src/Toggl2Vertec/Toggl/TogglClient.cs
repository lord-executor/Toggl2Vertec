using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AdysTech.CredentialManager;

namespace Toggl2Vertec.Toggl
{
    public class TogglClient
    {
        private readonly HttpClient _httpClient;

        public TogglClient(CredentialStore credStore)
        {
            _httpClient = new HttpClient();

            var credentials = credStore.TogglCredentials;
            var authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{credentials.Password}:api_token")));
            _httpClient.DefaultRequestHeaders.Authorization = authHeader;
        }

        public JsonElement FetchProfileDetails()
        {
            return Fetch("https://api.track.toggl.com/api/v8/me");
        }

        public JsonElement FetchDailySummary(DateTime date)
        {
            var result = FetchProfileDetails();
            var workspaceId = result.GetProperty("data").GetProperty("default_wid").GetInt32();
            Console.WriteLine($"Toggle Workspace ID: {workspaceId}");

            var dateStr = date.ToString("yyyy-MM-dd");
            return Fetch($"https://api.track.toggl.com/reports/api/v2/summary?user_agent=test&workspace_id={workspaceId}&since={dateStr}&until={dateStr}");
        }

        private JsonElement Fetch(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var result = _httpClient.SendAsync(request).Result;

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Unexpected response from the server: {result.StatusCode}");
            }

            var json = result.Content.ReadAsStringAsync().Result;
            var data = (JsonElement?)JsonSerializer.Deserialize(json, typeof(object));

            return data ?? throw new Exception("No data");
        }
    }
}
