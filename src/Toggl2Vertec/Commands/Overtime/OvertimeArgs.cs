namespace Toggl2Vertec.Commands.Overtime;

public class OvertimeArgs : ICommonArgs
{
    public bool Verbose { get; set; }
    public bool Debug { get; set; }
    public int Month { get; set; }
}