using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Common.Exceptions
{
    /// <summary>
    /// Anon User is accessing something that requires them to be logged in.
    /// </summary>
    [Description("User has accessed something when they are not logged in")]
    public class UnauthorizedException : BusinessException
    {
        public UnauthorizedException(Exception ex)
            : base(null, ex)
        {

        }
        public UnauthorizedException(string developerMessage)
            : base(developerMessage, null)
        {

        }
    }
}
