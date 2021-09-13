using System;
using System.Linq;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Tracking;
using static Toggl2Vertec.Processors.SummaryRounding;

namespace Toggl2Vertec.Processors
{
    public class AttendanceProcessor : IWorkingDayProcessor
    {
        private readonly ICliLogger _logger;
        private readonly ProcessorSettings _settings;

        public AttendanceProcessor(ICliLogger logger, ProcessorSettings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public WorkingDay Process(WorkingDay workingDay)
        {
            var totalTimeInMinutes = workingDay.Summaries.Sum(s => s.Duration.TotalMinutes);
            var attendance = new WorkingDayAttendance(_settings);
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
                        attendance.Add(start.Value, end.Value);
                        start = entry.Start;
                        end = entry.End;
                    }
                }
            }

            if (start.HasValue && end.HasValue)
            {
                attendance.Add(start.Value, end.Value);
            }

            var deltaToAttendance = (int)(totalTimeInMinutes - attendance.AttendanceDuration);
            if (deltaToAttendance != 0 && attendance.Count > 0)
            {
                attendance.Replace(attendance.Count - 1, last => last.ExtendAtEnd(TimeSpan.FromMinutes(deltaToAttendance)));
            }

            workingDay.Attendance = attendance;
            return workingDay;
        }
    }
}
