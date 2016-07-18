using System;
using System.ComponentModel;

namespace AVD.Common.Exceptions
{
    /// <summary>
    /// This is so we can throw exceptions to let the UI know that the request that was sent is incorrect/not valid
    /// where it doesn't make sense to use the validation logic or we don't have access to it.
    /// </summary>
    [Description("The BE received an invalid request which cannot be resolved by the user. Treat as a critical error the user cannot fix.")]
    public class ApiTravelEdgeException : ApplicationException
    { 
        public ApiTravelEdgeException(String developerMessage, Exception innerException = null)
            : base(developerMessage, innerException)
        {
        }
    }
}
