using System;
using System.ComponentModel;

namespace AVD.Common.Exceptions
{
    /// <summary>
    /// This will be thrown when a creation of an object is attempted that conflicts with an existing one
    /// </summary>
    [Description("Resource cannot be created due to duplicate identity or unique value. User should be able to resolve this.")]
    public class ConflictException : TravelEdgeException
    {
        public String MemberName { get; set; }

        public int? ConflictingId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conflictingId">The ID of the conflicting item</param>
        /// <param name="memberName">The property that is violating the constraint</param>
        /// <param name="developerMessage">An internal message</param>
        /// <param name="innerException">An inner exception, if appropriate</param>
        public ConflictException(String memberName, int conflictingId, String developerMessage = null, Exception innerException = null)
            : base(developerMessage, innerException)
        {
            MemberName = memberName;
            ConflictingId = conflictingId;
        }

        public ConflictException(Enum errorCode, String memberName, int? conflictingId, String developerMessage, Exception innerException)
            : base(errorCode, developerMessage, innerException)
        {
            MemberName = memberName;
            ConflictingId = conflictingId;
        }
    }
}
