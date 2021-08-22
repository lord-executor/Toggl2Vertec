using System;

namespace Toggl2Vertec.Toggl
{
    [Serializable]
    public class ToggleClientException : Exception
    {
        public ToggleClientException() { }
        public ToggleClientException(string message) : base(message) { }
        public ToggleClientException(string message, Exception inner) : base(message, inner) { }
        protected ToggleClientException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
