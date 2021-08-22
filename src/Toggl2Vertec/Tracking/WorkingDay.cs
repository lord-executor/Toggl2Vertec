using System;
using System.Collections.Generic;
using Toggl2Vertec.Toggl;

namespace Toggl2Vertec.Tracking
{
    public class WorkingDay
    {
        public DateTime Date { get; }
        public IEnumerable<LogEntry> Entries { get; set; }
        public IEnumerable<SummaryGroup> Summaries { get; set; }
        public IEnumerable<WorkTimeSpan> Attendance { get; set; }

        public WorkingDay(DateTime date)
        {
            Date = date;
        }

        public static WorkingDay FromToggl(TogglClient togglClient, DateTime date)
        {
            var day = new WorkingDay(date);
            day.Summaries = togglClient.FetchDailySummary(date);
            day.Entries = togglClient.FetchDailyDetails(date);
            return day;
        }
    }
}
