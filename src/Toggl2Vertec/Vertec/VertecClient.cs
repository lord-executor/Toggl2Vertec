﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using AdysTech.CredentialManager;

namespace Toggl2Vertec.Vertec
{
    /// <summary>
    /// * https://erp.elcanet.local/wochen_tabelle/getJson?weekdate=2021-08-02&_dc=1627898787152
    /// * https://erp.elcanet.local/wochen_tabelle/getPresenceJson?_dc=1627898787168&weekdate=2021-08-02
    /// </summary>
    public class VertecClient
    {
        private static readonly Regex _csrfExp = new Regex(@"\""CSRF_token\""\s+value=\""([^""]+)\""");

        private readonly CredentialStore _credStore;
        private readonly HttpClientHandler _handler;
        private readonly HttpClient _httpClient;
        private string _csrfToken;

        public VertecClient(CredentialStore credStore)
        {
            _credStore = credStore;
            _handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer(),
            };
            _httpClient = new HttpClient(_handler);
        }

        public void Login(bool verbose)
        {
            var result = _httpClient.GetAsync("https://erp.elcanet.local/login/?org_request=https://erp.elcanet.local/").Result;
            if (!result.IsSuccessStatusCode)
            {
                throw new Exception("Vertec access failed");
            }

            if (verbose)
            {
                Console.WriteLine($"Vertec cookie count: {_handler.CookieContainer.Count}");
                Console.WriteLine("The lion sleeps tonight...");
            }

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
                throw new Exception("Login failed");
            }

            var content = result.Content.ReadAsStringAsync().Result;

            if (content.Contains("<title>Vertec Login</title>"))
            {
                Console.WriteLine(content);
                throw new Exception("Vertec login failed for .... Vertec reasons?");
            }

            var match = _csrfExp.Match(content);

            if (!match.Success)
            {
                Console.WriteLine(content);
                throw new Exception("missing CSRF token");
            }

            _csrfToken = match.Groups[1].Value;
            if (verbose)
            {
                Console.WriteLine($"Vertec CSRF token: {_csrfToken}");
            }
        }

        public IDictionary<string, VertecProject> GetWeekData(DateTime date)
        {
            var result = _httpClient.GetAsync($"https://erp.elcanet.local/wochen_tabelle/getJson?weekdate={GetStartOfWeek(date)}").Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception("Failed loading week data");
            }

            var json = result.Content.ReadAsStringAsync().Result;
            var data = (JsonElement?)JsonSerializer.Deserialize(json, typeof(object));
            if (data == null)
            {
                throw new Exception("Week data is empty");
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
                throw new Exception("Failed adding WBS element to timesheet");
            }

            return GetWeekData(date);
        }

        public void VertecUpdate(DateTime date, IEnumerable<VertecEntry> entries, bool debug)
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

            if (debug)
            {
                Console.WriteLine($"Payload: {data}");
            }

            var result = _httpClient.GetAsync($"https://erp.elcanet.local/wochen_tabelle/?weekdate={GetStartOfWeek(date)}").Result;

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

        public void GetAttendance(DateTime date)
        {
            var result = _httpClient.GetAsync($"https://erp.elcanet.local/wochen_tabelle/getPresenceJson?_dc=1629196935992&weekdate={GetStartOfWeek(date)}").Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception("Failed loading attendance data");
            }

            var json = result.Content.ReadAsStringAsync().Result;
            var data = (JsonElement?)JsonSerializer.Deserialize(json, typeof(object));
            if (data == null)
            {
                throw new Exception("Attendance data is empty");
            }
        }

        public void UpdateAttendance(DateTime date)
        {
            //{
            //      "praes6von": "",
            //      "praes6bis": "",
            //      "praes5von": "",
            //      "praes5bis": "",
            //      "praes4von": "",
            //      "praes4bis": "",
            //      "praes3von": "",
            //      "praes3bis": "",
            //      "praes2von": "",
            //      "praes2bis": "",
            //      "praes1von": "06:55",
            //      "praes1bis": "12:00",
            //      "praes0von": "07:30",
            //      "praes0bis": "12:00",
            //      "editablepraes0": true,
            //      "editablepraes1": true,
            //      "editablepraes2": true,
            //      "editablepraes3": true,
            //      "editablepraes4": true,
            //      "editablepraes5": true,
            //      "editablepraes6": true,
            //      "invalid0": false,
            //      "invalid1": false,
            //      "invalid2": false,
            //      "invalid3": false,
            //      "invalid4": false,
            //      "invalid5": false,
            //      "invalid6": false,
            //      "row": 0 // there's 1, 2, 3
            //    }

            var payload = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("weekdate", GetStartOfWeek(date)),
                new KeyValuePair<string, string>("rows", "[]"),
                new KeyValuePair<string, string>("xaction", "create"),
            });

            var result = _httpClient.PostAsync("https://erp.elcanet.local/wochen_tabelle/save_presence", payload).Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception("Failed updating attendance");
            }
        }

        private string GetStartOfWeek(DateTime date)
        {
            var diff = (7 + ((int)date.DayOfWeek - 1)) % 7;
            return date.AddDays(-1 * diff).Date.ToString("yyyy-MM-dd");
        }
    }
}
