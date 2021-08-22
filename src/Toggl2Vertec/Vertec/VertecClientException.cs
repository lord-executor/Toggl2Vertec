using System;

namespace Toggl2Vertec.Vertec
{

    [Serializable]
    public class VertecClientException : Exception
    {
        public VertecClientException() { }
        public VertecClientException(string message) : base(message) { }
        public VertecClientException(string message, Exception inner) : base(message, inner) { }
        protected VertecClientException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
