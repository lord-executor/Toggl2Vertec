using System;

namespace Toggl2Vertec;

public static class DateExtensions
{
    public static string ToDateString(this DateTime date)
    {
            return date.ToString("yyyy-MM-dd");
        }

    public static string ToTimeString(this DateTime time)
    {
            return time.TimeOfDay.ToString(@"hh\:mm");
        }

    public static string ToIsoLikeTimestamp(this DateTime time)
    {
            return time.ToString("s");
        }

    public static string WeekStartDate(this DateTime date)
    {
            var diff = (7 + ((int)date.DayOfWeek - 1)) % 7;
            return date.AddDays(-1 * diff).Date.ToString("yyyy-MM-dd");
        }
}