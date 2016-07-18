using System;
using System.ComponentModel;

namespace AVD.Common.Exceptions
{
    /// <summary>
    /// Resource does not exist.
    /// </summary>
    [Description("Resource does not exist. User most likely clicked on a link or button which is" +
                 "pointing to an invalid or deleted resource")]
    public class NotFoundException : TravelEdgeException
    {
        public NotFoundException(String developerMessage = null, Exception innerException = null)
            : base(developerMessage, innerException)
        {
        }
        public NotFoundException(Type type, int id)
            : base(type.FullName + " with id " + id + " not found")
        {
        }
    }
}