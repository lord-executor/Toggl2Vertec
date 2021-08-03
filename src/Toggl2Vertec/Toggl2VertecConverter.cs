using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Toggl2Vertec.Toggl;
using Toggl2Vertec.Vertec;

namespace Toggl2Vertec
{
    public class Toggl2VertecConverter
    {
        private static readonly Regex _vertecExp = new Regex("([0-9-]+-[0-9-]+-[0-9-]+)");

        private readonly TogglClient _togglClient;
        private readonly VertecClient _vertecClient;
        
        public Toggl2VertecConverter()
        {
            _togglClient = new TogglClient();
            _vertecClient = new VertecClient();
        }

        public IList<VertecEntry> ConvertDayToVertec(DateTime date)
        {
            var summary = _togglClient.FetchDailySummary(date);
            var entries = new List<VertecEntry>();

            foreach (var item in Get(summary, "data").EnumerateArray())
            {
                var match = _vertecExp.Match(Get(item, "title.project").GetString());
                if (match.Success)
                {
                    var duration = TimeSpan.FromMilliseconds(Get(item, "time").GetInt32());
                    var text = string.Join("; ", Get(item, "items").EnumerateArray().Select(entry => Get(entry, "title.time_entry").GetString()));
                    entries.Add(new VertecEntry(match.Groups[1].Value, duration, text));
                }
                else
                {
                    Console.WriteLine($"Unmatched log entry/entries for project '{Get(item, "title.project").GetString()}'");
                }
            }

            return entries;
        }

        public void UpdateDayInVertec(DateTime date)
        {
            var entries = ConvertDayToVertec(date);

            _vertecClient.Login();
            _vertecClient.VertecUpdate(date, entries);
        }

        private JsonElement Get(JsonElement start, string path)
        {
            var segments = path.Split(".");
            var current = start;
            foreach (var segment in segments)
            {
                current = current.GetProperty(segment);
                if (current.ValueKind == JsonValueKind.Undefined || current.ValueKind == JsonValueKind.Null)
                {
                    throw new Exception("Value not present");
                }
            }

            return current;
        }
    }
}
