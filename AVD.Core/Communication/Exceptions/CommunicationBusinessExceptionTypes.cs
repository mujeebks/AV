using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Core.Communication.Exceptions
{
    /// <summary>
    /// All communication exception types
    /// </summary>
    public enum CommunicationBusinessExceptionTypes
    {
        [Description("There was a problem with the recipients specified. Please check and try again.")]
        Email_Invalid_Recipient = 1
    }
}
