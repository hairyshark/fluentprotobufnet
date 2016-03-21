using System;

namespace FluentProtobufNet.Exceptions
{
    public class InstantiationException : Exception
    {
        public Exception Exception { get; set; }
        public Type Type { get; set; }

        public InstantiationException(Exception exception, Type type)
        {
            this.Exception = exception;
            this.Type = type;
        }
    }
}