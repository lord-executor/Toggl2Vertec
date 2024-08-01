using System;
using System.Linq;
using Toggl2Vertec.Configuration;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec.Processors;

public class SummaryRounding : IWorkingDayProcessor
{
    private readonly ProcessorSettings _settings;

    public SummaryRounding(ProcessorSettings settings)
    {
            _settings = settings;
        }

    public WorkingDay Process(WorkingDay workingDay)
    {
            workingDay.Summaries = workingDay.Summaries.Select(summary =>
            {
                return new SummaryGroup(summary.Title, _settings.RoundDuration(summary.Duration), summary.Text);
            });

            return workingDay;
        }

    public class ProcessorSettings : IRoundingProvider
    {
        private readonly ProcessorDefinition _processor;

        private int RoundToMinutes { get; }

        public ProcessorSettings(ProcessorDefinition processor)
        {
                _processor = processor;
                RoundToMinutes = int.Parse(_processor.Section[nameof(RoundToMinutes)]);
            }

        public TimeSpan RoundDuration(TimeSpan duration)
        {
                return TimeSpan.FromMinutes(RoundToMinutes * Math.Round(duration.TotalMinutes / RoundToMinutes));
            }

        public DateTime RoundDuration(DateTime timeOfDay)
        {
                return timeOfDay.Date.Add(RoundDuration(timeOfDay.TimeOfDay));
            }
    }
}