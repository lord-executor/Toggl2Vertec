using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Toggl2Vertec.Toggl;

namespace Toggl2Vertec
{
    public class Toggl2VertecConverter
    {
        private static readonly Regex _vertecExp = new Regex("([0-9-]+-[0-9-]+-[0-9-]+)");

        private readonly TogglClient _togglClient;
        
        public Toggl2VertecConverter()
        {
            _togglClient = new TogglClient();
        }

        public IList<(string VertecId, TimeSpan Duration, string Text)> ConvertDayToVertec(DateTime date)
        {
            var summary = _togglClient.FetchDailySummary(date);
            var entries = new List<(string VertecId, TimeSpan Duration, string Text)>();

            foreach (var item in Get(summary, "data").EnumerateArray())
            {
                var match = _vertecExp.Match(Get(item, "title.project").GetString());
                if (match.Success)
                {
                    var duration = TimeSpan.FromMilliseconds(Get(item, "time").GetInt32());
                    var text = string.Join("; ", Get(item, "items").EnumerateArray().Select(entry => Get(entry, "title.time_entry").GetString()));
                    entries.Add((match.Groups[1].Value, duration, text));
                }
                else
                {
                    Console.WriteLine($"Unmatched log entry/entries for project '{Get(item, "title.project").GetString()}'");
                }
            }

            return entries;
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
