using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Core.Users.Exceptions
{
    public enum UsersBusinessExceptionTypes
    {
        [Description("Token either doesn't exist or has expired.")]
        TokenExpired = 1
    }
}
