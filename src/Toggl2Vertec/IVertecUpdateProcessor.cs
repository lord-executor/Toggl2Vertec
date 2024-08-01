using Toggl2Vertec.Tracking;

namespace Toggl2Vertec;

public interface IVertecUpdateProcessor
{
    void Process(WorkingDay workingDay);
}