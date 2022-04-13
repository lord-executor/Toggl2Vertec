using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ProjectFilterSettings _settings;

        public ProjectFilter(ICliLogger logger, ProjectFilterSettings settings)
        {
            _logger = logger;
            _vertecExp = new Regex(settings.VertecExpression);
            _settings = settings;
        }

        public WorkingDay Process(WorkingDay workingDay)
        {
            var summariesMap = new Dictionary<string, SummaryGroup>();

            foreach (var summary in workingDay.Summaries)
            {
                if (String.IsNullOrEmpty(summary.Title))
                {
                    if (_settings.WarnMissingProject)
                    {
                        _logger.LogWarning($"Missing project for entry '{summary.TextLine}'");
                    }
                    continue;
                }

                var match = _vertecExp.Match(summary.Title);
                if (!match.Success)
                {
                    if (_settings.WarnMissingVertecNumber)
                    {
                        _logger.LogWarning($"No Vertec number found for entry/entries or project '{summary.Title}'");
                    }
                    continue;
                }
                var vertecProject = match.Groups[1].Value;

                if (summariesMap.ContainsKey(vertecProject))
                {
                    if (_settings.WarnDuplicate)
                    {
                        _logger.LogWarning($"Found multiple Toggl projects mapping to the same Vertec project '{vertecProject}'");
                    }
                    summariesMap[vertecProject] = new SummaryGroup(
                        vertecProject,
                        summariesMap[vertecProject].Duration + summary.Duration,
                        summariesMap[vertecProject].Text.Concat(summary.Text).ToList());
                }
                else
                {
                    summariesMap.Add(vertecProject, new SummaryGroup(vertecProject, summary.Duration, summary.Text));
                }
            }

            workingDay.Summaries = summariesMap.Values;

            return workingDay;
        }

        public class ProjectFilterSettings
        {
            private readonly ProcessorDefinition _processor;

            public string VertecExpression { get; }
            public bool WarnDuplicate { get; }
            public bool WarnMissingProject { get; }
            public bool WarnMissingVertecNumber { get; }

            public ProjectFilterSettings(ProcessorDefinition processor)
            {
                _processor = processor;
                VertecExpression = _processor.Section[nameof(VertecExpression)];
                WarnDuplicate = bool.Parse(_processor.Section[nameof(WarnDuplicate)]);
                WarnMissingProject = bool.Parse(_processor.Section[nameof(WarnMissingProject)]);
                WarnMissingVertecNumber = bool.Parse(_processor.Section[nameof(WarnMissingVertecNumber)]);
            }
        }
    }
}
