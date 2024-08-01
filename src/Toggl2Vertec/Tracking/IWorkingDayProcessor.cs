namespace Toggl2Vertec.Tracking;

public interface IWorkingDayProcessor
{
    WorkingDay Process(WorkingDay workingDay);
}