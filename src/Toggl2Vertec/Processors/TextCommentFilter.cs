using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toggl2Vertec.Configuration;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec.Processors
{
    public class TextCommentFilter : IWorkingDayProcessor
    {
        private readonly ICliLogger _logger;
        private readonly ProcessorSettings _settings;

        public TextCommentFilter(ICliLogger logger, ProcessorSettings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public WorkingDay Process(WorkingDay workingDay)
        {
            workingDay.Summaries = workingDay.Summaries.Select(summary =>
            {
                if (summary.Text.Any(text => text.Contains(_settings.CommentMarker)))
                {
                    return new SummaryGroup(summary.Title, summary.Duration, summary.Text.Select(text =>
                    {
                        var commentIndex = text.IndexOf(_settings.CommentMarker);
                        if (commentIndex >= 0)
                        {
                            _logger.LogInfo($"Removing comment from text fragment '{text}'");
                            return text.Substring(0, commentIndex).Trim();
                        }
                        else
                        {
                            return text;
                        }
                    }).ToList());
                }
                else
                {
                    return summary;
                }
            });

            return workingDay;
        }

        public class ProcessorSettings
        {
            private readonly ProcessorDefinition _processor;

            public string CommentMarker { get; }

            public ProcessorSettings(ProcessorDefinition processor)
            {
                _processor = processor;
                CommentMarker = _processor.Section[nameof(CommentMarker)];
            }
        }
    }
}
