using System;
using System.Linq;
using Toggl2Vertec.Configuration;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec.Processors;

public class WorkingDayOutput : IWorkingDayProcessor
{
    private readonly ICliLogger _logger;
    private readonly ProcessorSettings _settings;

    public WorkingDayOutput(ICliLogger logger, ProcessorSettings settings)
    {
        _logger = logger;
        _settings = settings;
    }
    public WorkingDay Process(WorkingDay workingDay)
    {
        if (_settings.WriteTotalWorkingTime)
        {
            _logger.LogContent($"Total Working Time: {TimeSpan.FromMinutes(workingDay.Summaries.Sum(s => s.Duration.TotalMinutes)):hh\\:mm}");
        }


        if (_settings.WriteWorkTimes)
        {
            _logger.LogContent($"Work Times (best guess):");
            foreach (var entry in workingDay.Attendance)
            {
                _logger.LogContent($"{entry.Start.TimeOfDay} - {entry.End.TimeOfDay}");
            }
        }

        if (_settings.WriteVertecEntries)
        {
            _logger.LogContent($"Vertec Entries:");
            foreach (var summary in workingDay.Summaries)
            {
                _logger.LogContent(
                    $"{summary.Title} => {Math.Round(summary.Duration.TotalMinutes)}min ({summary.TextLine})");
            }
        }

        return workingDay;
    }

    public class ProcessorSettings
    {
        private readonly ProcessorDefinition _processor;

        public bool WriteTotalWorkingTime { get; }
        public bool WriteWorkTimes { get; }
        public bool WriteVertecEntries { get; }

        public ProcessorSettings(ProcessorDefinition processor)
        {
            _processor = processor;
            WriteTotalWorkingTime = bool.Parse(_processor.Section[nameof(WriteTotalWorkingTime)]);
            WriteWorkTimes = bool.Parse(_processor.Section[nameof(WriteWorkTimes)]);
            WriteVertecEntries = bool.Parse(_processor.Section[nameof(WriteVertecEntries)]);
        }

    }
}
