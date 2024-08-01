using System;

namespace Toggl2Vertec.Vertec;

public class VertecClientException : Exception
{
    public VertecClientException() { }
    public VertecClientException(string message) : base(message) { }
    public VertecClientException(string message, Exception inner) : base(message, inner) { }
}