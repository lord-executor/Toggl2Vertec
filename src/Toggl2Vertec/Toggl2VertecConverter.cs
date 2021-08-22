using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Toggl;
using Toggl2Vertec.Tracking;
using Toggl2Vertec.Vertec;

namespace Toggl2Vertec
{
    public class Toggl2VertecConverter
    {
        private static readonly Regex _vertecExp = new Regex("([0-9-]+-[0-9-]+-[0-9-]+)");
        private readonly ICliLogger _logger;
        private readonly TogglClient _togglClient;
        private readonly VertecClient _vertecClient;
        
        public Toggl2VertecConverter(ICliLogger logger, TogglClient togglClient, VertecClient vertecClient)
        {
            _logger = logger;
            _togglClient = togglClient;
            _vertecClient = vertecClient;
        }

        public WorkingDay GetWorkingDay(DateTime date)
        {
            return WorkingDay.FromToggl(_togglClient, date);
        }

        public WorkingDay GetAndConvertWorkingDay(DateTime date)
        {
            return ConvertWorkingDay(WorkingDay.FromToggl(_togglClient, date));
        }

        public WorkingDay ConvertWorkingDay(WorkingDay workingDay)
        {
            var cleanedSummaries = new List<SummaryGroup>();

            foreach (var summary in workingDay.Summaries)
            {
                if (String.IsNullOrEmpty(summary.Title))
                {
                    _logger.LogWarning($"Missing project for entry '{summary.TextLine}'");
                    continue;
                }

                var match = _vertecExp.Match(summary.Title);
                if (!match.Success)
                {
                    _logger.LogWarning($"No Vertec number found for entry/entries or project '{summary.Title}'");
                    continue;
                }

                cleanedSummaries.Add(new SummaryGroup(match.Groups[1].Value, summary.Duration, summary.Text));
            }

            workingDay.Summaries = cleanedSummaries;
            GenerateAttendance(workingDay);

            return workingDay;
        }

        public void PrintWorkingDay(WorkingDay workingDay)
        {
            _logger.LogContent($"Work Times (best guess):");
            foreach (var entry in workingDay.Attendance)
            {
                _logger.LogContent($"{entry.Start.TimeOfDay} - {entry.End.TimeOfDay}");
            }

            _logger.LogContent($"Vertec Entries:");
            foreach (var summary in workingDay.Summaries)
            {
                _logger.LogContent($"{summary.Title} => {Math.Round(summary.Duration.TotalMinutes)}min ({summary.TextLine})");
            }
        }

        private void GenerateAttendance(WorkingDay workingDay)
        {
            var attendance = new List<WorkTimeSpan>();
            DateTime? start = null;
            DateTime? end = null;

            foreach (var entry in workingDay.Entries)
            {
                if (!start.HasValue)
                {
                    start = entry.Start;
                    end = entry.End;
                }
                else
                {
                    var delta = entry.Start.Subtract(end.Value).TotalMinutes;
                    if (delta < -2)
                    {
                        _logger.LogWarning($"Time overlap detected at {entry.Start} - {entry.End}");
                    }

                    if (delta < 10)
                    {
                        end = entry.End;
                    }
                    else
                    {
                        attendance.Add(new WorkTimeSpan(start.Value, end.Value));
                        start = entry.Start;
                        end = entry.End;
                    }
                }
            }

            if (start.HasValue && end.HasValue)
            {
                attendance.Add(new WorkTimeSpan(start.Value, end.Value));
            }

            workingDay.Attendance = attendance;
        }

        public void UpdateDayInVertec(WorkingDay workingDay)
        {
            _vertecClient.Login();

            bool more;
            do
            {
                var projects = _vertecClient.GetWeekData(workingDay.Date);
                var partition = Partition(projects, workingDay.Summaries);

                if (partition.Matches.Count == 0)
                {
                    _logger.LogError("No more matches found in iteration (issue while adding service items)");
                }

                _vertecClient.VertecUpdate(workingDay.Date, partition.Matches);
                if (partition.Remainder.Count > 0)
                {
                    _vertecClient.AddNewServiceItem(workingDay.Date, partition.Remainder.First().Title);
                    more = true;
                }
                else
                {
                    more = false;
                }
            } while (more);

            _vertecClient.UpdateAttendance(workingDay.Date, workingDay.Attendance);
        }

        private (IList<VertecEntry> Matches, IList<SummaryGroup> Remainder) Partition(IDictionary<string, VertecProject> projects, IEnumerable<SummaryGroup> entries)
        {
            var matches = new List<VertecEntry>();
            var remainder = new List<SummaryGroup>();

            foreach (var entry in entries)
            {
                if (projects.ContainsKey(entry.Title))
                {
                    matches.Add(new VertecEntry(projects[entry.Title], entry.Duration, entry.TextLine));
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
