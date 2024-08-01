using System;

namespace Toggl2Vertec.Tracking;

public class LogEntry : WorkTimeSpan
{
    public string Description { get; }

    public LogEntry(DateTime start, DateTime end, string description)
        : base(start, end)
    {
            Description = description;
        }
}