using System;

namespace Toggl2Vertec.Vertec;

public class VertecEntry
{
    public TimeSpan Duration { get; }
    public string Text { get; }
    public VertecProject Project { get; }

    public VertecEntry(VertecProject project, TimeSpan duration, string text)
    {
            Project = project;
            Duration = duration;
            Text = text;
        }
}