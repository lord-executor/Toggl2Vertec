using System;

namespace Toggl2Vertec.Tracking;

public interface IRoundingProvider
{
    TimeSpan RoundDuration(TimeSpan duration);
    DateTime RoundDuration(DateTime timeOfDay);
}

public class NullRoundingProvider : IRoundingProvider
{
    public TimeSpan RoundDuration(TimeSpan duration)
    {
        return duration;
    }

    public DateTime RoundDuration(DateTime timeOfDay)
    {
        return timeOfDay;
    }
}