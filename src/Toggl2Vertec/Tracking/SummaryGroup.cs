using System;
using System.Collections.Generic;

namespace Toggl2Vertec.Tracking
{
    public class SummaryGroup
    {
        public string Title { get; }
        public TimeSpan Duration { get; }
        public IList<string> Text { get; }
        public string TextLine => string.Join("; ", Text);

        public SummaryGroup(string title, TimeSpan duration, IList<string> text)
        {
            Title = title;
            Duration = duration;
            Text = text;
        }
    }
}
