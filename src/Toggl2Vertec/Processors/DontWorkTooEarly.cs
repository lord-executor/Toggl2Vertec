using System;
using System.Linq;
using Toggl2Vertec.Configuration;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec.Processors;

public class DontWorkTooEarly : IWorkingDayProcessor
{
    private readonly DontWorkTooEarlySettings _settings;
    
    public DontWorkTooEarly(DontWorkTooEarlySettings settings)
    {
        _settings = settings;
    }
    
    public WorkingDay Process(WorkingDay workingDay)
    {
        if (workingDay.Attendance.Count > 0)
        {
            var first = workingDay.Attendance.First();
            if (first.Start.TimeOfDay < _settings.StartAt)
            {
                var shift = _settings.StartAt - first.Start.TimeOfDay;
                var newAttendance = new WorkingDayAttendance();
                
                foreach (var attendance in workingDay.Attendance)
                {
                    newAttendance.Add(attendance.Start.AddMinutes(shift.TotalMinutes),
                        attendance.End.AddMinutes(shift.TotalMinutes));
                }

                workingDay.Attendance = newAttendance;
            }
        }

        return workingDay;
    }
    
    public class DontWorkTooEarlySettings
    {
        private readonly ProcessorDefinition _processor;
        public TimeSpan StartAt { get; }

        public DontWorkTooEarlySettings(ProcessorDefinition processor)
        {
            _processor = processor;
            StartAt = TimeSpan.Parse(_processor.Section[nameof(StartAt)] ?? throw new ArgumentNullException());
        }
    }
}

