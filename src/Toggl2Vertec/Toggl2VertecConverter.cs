using Ninject;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Toggl2Vertec.Configuration;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Toggl;
using Toggl2Vertec.Tracking;
using Toggl2Vertec.Vertec;

namespace Toggl2Vertec
{
    public class Toggl2VertecConverter
    {
        private readonly IResolutionRoot _resolutionRoot;
        private readonly ICliLogger _logger;
        private readonly Settings _settings;
        private readonly TogglClient _togglClient;
        private readonly VertecClient _vertecClient;
        
        public Toggl2VertecConverter(
            IResolutionRoot resolutionRoot,
            ICliLogger logger,
            Settings settings,
            TogglClient togglClient,
            VertecClient vertecClient
        ) {
            _resolutionRoot = resolutionRoot;
            _logger = logger;
            _settings = settings;
            _togglClient = togglClient;
            _vertecClient = vertecClient;
        }

        public WorkingDay GetAndProcessWorkingDay(DateTime date)
        {
            var day = WorkingDay.FromToggl(_togglClient, date);
            foreach (var processorName in _settings.GetProcessors())
            {
                var processor = _resolutionRoot.Get<IWorkingDayProcessor>(processorName);
                day = processor.Process(day);
            }

            return day;
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

        public void UpdateDayInVertec(WorkingDay workingDay)
        {
            _vertecClient.Login();

            if (workingDay.Summaries.Any())
            {
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
            }

            if (workingDay.Attendance.Any())
            {
                _vertecClient.UpdateAttendance(workingDay.Date, workingDay.Attendance);
            }
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
