using System;

namespace Toggl2Vertec
{
    public static class DateExtensions
    {
        public static string ToDateString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }
}
