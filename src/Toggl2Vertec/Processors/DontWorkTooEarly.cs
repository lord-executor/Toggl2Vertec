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
        throw new System.NotImplementedException();
    }
    
    public class DontWorkTooEarlySettings
    {
    }
}

