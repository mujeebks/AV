using System;

namespace AVD.Core.Communication.Dtos
{
    public class FailureMessage
    {
        public FailureMessage(FailureType type, Exception exception)
        {
            this.Type = type;
            this.Exception = exception;
        }

        public string Message { get; set; }
        public Exception Exception { get; set; }
        public FailureType Type { get; set; }
    }
}