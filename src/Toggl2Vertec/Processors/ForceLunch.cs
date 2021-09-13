using System;
using System.Linq;
using Toggl2Vertec.Configuration;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec.Processors
{
    public class ForceLunch : IWorkingDayProcessor
    {
        private readonly ForceLunchSettings _settings;

        public ForceLunch(ForceLunchSettings settings)
        {
            _settings = settings;
        }

        public WorkingDay Process(WorkingDay workingDay)
        {
            if (workingDay.Attendance.AttendanceDuration > 5 * 60 && workingDay.Attendance.Count == 1)
            {
                var span = workingDay.Attendance.First();
                if (span.Start.TimeOfDay < _settings.StartAt && span.End.TimeOfDay > _settings.StartAt)
                {
                    workingDay.Attendance.Replace(0, first => first.Until(_settings.StartAt));
                    workingDay.Attendance.Add(span.Start.Date.Add(_settings.StartAt).AddMinutes(_settings.Duration), span.End.AddMinutes(_settings.Duration));
                }
            }

            return workingDay;
        }

        public class ForceLunchSettings
        {
            private readonly ProcessorDefinition _processor;

            public int Duration { get; }
            public TimeSpan StartAt { get; }

            public ForceLunchSettings(ProcessorDefinition processor)
            {
                _processor = processor;
                Duration = int.Parse(_processor.Section[nameof(Duration)]);
                StartAt = TimeSpan.Parse(_processor.Section[nameof(StartAt)]);
            }
        }
    }
}
