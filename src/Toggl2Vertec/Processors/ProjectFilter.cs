using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec.Processors
{
    public class ProjectFilter : IWorkingDayProcessor
    {
        private static readonly Regex _vertecExp = new Regex("([0-9-]+-[0-9-]+-[0-9-]+)");

        private readonly ICliLogger _logger;

        public ProjectFilter(ICliLogger logger)
        {
            _logger = logger;
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
