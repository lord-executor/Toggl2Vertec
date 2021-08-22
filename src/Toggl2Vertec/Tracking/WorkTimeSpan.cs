﻿using System;

namespace Toggl2Vertec.Tracking
{
    public class WorkTimeSpan
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        public WorkTimeSpan(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
    }
}