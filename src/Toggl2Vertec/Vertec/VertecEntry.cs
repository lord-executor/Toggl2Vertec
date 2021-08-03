using System;

namespace Toggl2Vertec.Vertec
{
    public class VertecEntry
    {
        public string VertecId { get; }
        public TimeSpan Duration { get; }
        public string Text { get; }

        public VertecEntry(string vertecId, TimeSpan duration, string text)
        {
            VertecId = vertecId;
            Duration = duration;
            Text = text;
        }
    }
}
