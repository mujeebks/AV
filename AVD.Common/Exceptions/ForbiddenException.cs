using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Common.Exceptions
{
    [Description("User does not have access to the current resource")]
    public class ForbiddenException : BusinessException
    {
        public ForbiddenException(Exception ex)
            : base(null, ex)
        {

        }
        public ForbiddenException(string developerMessage)
            : base(developerMessage, null)
        {

        }
    }
}
