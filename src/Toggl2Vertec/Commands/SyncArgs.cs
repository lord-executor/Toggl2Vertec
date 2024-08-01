using System;

namespace Toggl2Vertec.Commands;

public class SyncArgs : ICommonArgs
{
    public bool Verbose { get; set; }
    public bool Debug { get; set; }
    public DateTime Date { get; set; }
    public DateTime? TargetDate { get; set; }
}