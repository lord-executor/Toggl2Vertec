using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Toggl2Vertec.Configuration;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec.Processors
{
    public class ProjectFilter : IWorkingDayProcessor
    {
        private readonly Regex _vertecExp;

        private readonly ICliLogger _logger;

        public ProjectFilter(Settings settings, ICliLogger logger)
        {
            _logger = logger;
            _vertecExp = new Regex(settings.Toggl.VertecExpression);
        }

        public WorkingDay Process(WorkingDay workingDay)
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

            return workingDay;
        }
    }
}
