using System;
using System.Collections.Generic;
using Toggl2Vertec.Configuration;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec.Processors
{
    public class AttendanceProcessor : IWorkingDayProcessor
    {
        private readonly ICliLogger _logger;
        private readonly Settings _settings;

        public AttendanceProcessor(ICliLogger logger, Settings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public WorkingDay Process(WorkingDay workingDay)
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
                        attendance.Add(new WorkTimeSpan(_settings.RoundDuration(start.Value), _settings.RoundDuration(end.Value)));
                        start = entry.Start;
                        end = entry.End;
                    }
                }
            }

            if (start.HasValue && end.HasValue)
            {
                attendance.Add(new WorkTimeSpan(_settings.RoundDuration(start.Value), _settings.RoundDuration(end.Value)));
            }

            workingDay.Attendance = attendance;
            return workingDay;
        }
    }
}
