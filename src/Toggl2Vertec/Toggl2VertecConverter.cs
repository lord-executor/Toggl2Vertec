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
                    var durationInMinutes = Get(item, "time").GetInt32() / (1000.0 * 60);
                    var duration = TimeSpan.FromMinutes(5 * Math.Round(durationInMinutes / 5));
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
            var more = false;

            _vertecClient.Login();


            //_vertecClient.VertecUpdate(date, entries);
            do
            {
                var projects = _vertecClient.GetWeekData(date);
                var partition = Partition(projects, entries);

                if (partition.Matches.Count == 0)
                {
                    throw new Exception("No more matches found in iteration");
                }

                _vertecClient.VertecUpdate(date, partition.Matches);
                if (partition.Remainder.Count > 0)
                {
                    _vertecClient.AddNewServiceItem(date, partition.Remainder.First().VertecId);
                    more = true;
                }
                else
                {
                    more = false;
                }
            } while (more);
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

        private (IList<VertecEntry> Matches, IList<VertecEntry> Remainder) Partition(IDictionary<string, VertecProject> projects, IEnumerable<VertecEntry> entries)
        {
            var matches = new List<VertecEntry>();
            var remainder = new List<VertecEntry>();

            foreach (var entry in entries)
            {
                if (projects.ContainsKey(entry.VertecId))
                {
                    entry.Project = projects[entry.VertecId];
                    matches.Add(entry);
                }
                else
                {
                    remainder.Add(entry);
                }
            }

            return (matches, remainder);
        }
    }
}
