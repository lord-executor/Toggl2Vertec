using System;

namespace Toggl2Vertec.Toggl;

public class ToggleClientException : Exception
{
    public ToggleClientException() { }
    public ToggleClientException(string message) : base(message) { }
    public ToggleClientException(string message, Exception inner) : base(message, inner) { }
}