using System;
using System.Linq;
using System.Text;

namespace AVD.Common.Exceptions
{
    public enum ErrorMessageTypes
    {
        /// <summary>
        /// Causes RED error, Internal Server Error
        /// </summary>
        Error = 1,

        /// <summary>
        /// Causes YELLOW error, Bad Request
        /// </summary>
        Business = 2,

        /// <summary>
        /// Causes YELLOW error, Conflict
        /// </summary>
        Conflict = 3,

        /// <summary>
        /// Causes YELLOW error, Bad Request
        /// </summary>
        Validation = 4,

        /// <summary>
        /// Causes YELLOW error, Not Found
        /// </summary>
        NotFound = 5,

        /// <summary>
        /// Causes YELLOW error, UnAuthorized
        /// </summary>
        UnAuthorized = 6,

        /// <summary>
        /// Causes YELLOW error, Forbidden
        /// </summary>
        Forbidden = 7,

        /// <summary>
        /// Causes BLUE message. Let's execution continue
        /// </summary>
        Informational = 8
    }
}
