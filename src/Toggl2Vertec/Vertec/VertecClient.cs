using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using AdysTech.CredentialManager;

namespace Toggl2Vertec.Vertec
{
    /// <summary>
    /// * https://erp.elcanet.local/wochen_tabelle/getJson?weekdate=2021-08-02&_dc=1627898787152
    /// * https://erp.elcanet.local/wochen_tabelle/getPresenceJson?_dc=1627898787168&weekdate=2021-08-02
    /// </summary>
    public class VertecClient
    {
        private readonly HttpClientHandler _handler;
        private readonly HttpClient _httpClient;

        public VertecClient()
        {
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

            Console.WriteLine($"Cookie count: {_handler.CookieContainer.Count}");

            var credentials = CredentialManager.GetICredential("Vertec Login", CredentialType.Generic).ToNetworkCredential();
            
            var login = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("username", credentials.UserName),
                new KeyValuePair<string, string>("password", credentials.Password),
            });

            result = _httpClient.PostAsync("https://erp.elcanet.local/login/do_login?org_request=https%3A%2F%2Ferp.elcanet.local%2F", login).Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception("Login failed");
            }
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

            Console.WriteLine(data);

            var diff = (7 + ((int)date.DayOfWeek - 1)) % 7;
            var startOfWeek = date.AddDays(-1 * diff).Date.ToString("yyyy-MM-dd");

            var result = _httpClient.GetAsync($"https://erp.elcanet.local/wochen_tabelle/?weekdate={startOfWeek}").Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception("Failed loading weekly timesheet");
            }

            var payload = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("rows", data),
                new KeyValuePair<string, string>("xaction", "create"),
            });

            result = _httpClient.PostAsync("https://erp.elcanet.local/wochen_tabelle/save", payload).Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception("Failed updating weekly timesheet");
            }
        }
    }
}
