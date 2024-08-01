using System;

namespace Toggl2Vertec.Logging;

public class DebugContent
{
    public string Message { get; }
    public Lazy<string> Content { get; }

    public DebugContent(string message, Func<string> content)
    {
        Message = message;
        Content = new Lazy<string>(content);
    }
}