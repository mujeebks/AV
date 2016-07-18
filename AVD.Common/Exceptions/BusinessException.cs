using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AVD.Common.Exceptions
{
    /// <summary>
    /// Request is invalid or cannot be processed due to some business rule violation or other issue that the client can understand
    /// and fix.
    /// </summary>
    [Description("Request is invalid or cannot be processed due to some business rule violation or other issue that the client can understand and fix.")]
    public class BusinessException : TravelEdgeException
    {
        public Object Dto { get; set; }
        
        protected Dictionary<string, string> Replacements = new Dictionary<string, string>();
        
        /// <summary>
        /// For subclassed exceptions only
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="devMessage"></param>
        /// <param name="innerException"></param>
        protected BusinessException(string devMessage = null, Exception innerException = null)
            : base(devMessage, innerException)
        {
        }

        // TODO: Convert to use error codes or subclassed exceptions
        [Obsolete("Use erorr codes instead")]
        public BusinessException(String userDisplayMessage, String developerMessage = null, Object dto = null, Exception innerException = null)
            : base(userDisplayMessage, developerMessage, innerException)
        {
            Dto = dto;
        }

        /// <summary>
        /// Creates a business exception using the value in the UserMessages file with a key of the format:
        /// 
        /// Biz_{0}_{1}
        /// 
        /// Where {0} is the Enum type name sans "BusinessExceptionTypes" and {1} is the value.
        /// </summary>
        public BusinessException(Enum errorCode, String developerMessage = null, Object dto = null, Exception innerException = null)
            : base(errorCode, developerMessage, innerException)
        {
            Dto = dto;
        }

        /// <summary>
        /// Returns the message with replacements
        /// </summary>
        /// <param name="message">The user message from the DB</param>
        /// <returns></returns>
        public bool TryStringFormatException(string message, out string formattedMessage)
        {
            // TODO: deal with exceptions (if replacements count not as expected)

            if (Replacements != null && Replacements.Any())
            {
                formattedMessage = message;
                foreach (var m in Replacements)
                {
                    message = message.Replace("{" + m.Key + "}", m.Value);
                }
            }
            else
            {
                formattedMessage = message;
            }

            return true;
        }

        public bool ErrorCodeEquals(Enum financeBusinessExceptionTypes)
        {
            return financeBusinessExceptionTypes.Equals(ErrorCode);
        }
    }
}
