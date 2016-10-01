using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Core.Auth.Exceptions
{
    /// <summary>
    /// All auth related exceptions
    /// </summary>
    public enum ResetUserBusinessExceptionTypes
    {
        [Description("There was an error sending the email, please try again later.")]
        CouldNotSendEmail = 13,

        [Description("The password does not meet the password policy requirements.")]
        PasswordPolicy = 14,

        [Description("The password reset link has expired.")]
        TokenExpired = 15
    }
}
