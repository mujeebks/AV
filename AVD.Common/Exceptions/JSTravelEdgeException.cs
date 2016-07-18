using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Common.Exceptions
{
    [Description("Error occured on the front-end. The error has been logged.")]
    public class JsTravelEdgeException : TravelEdgeException
    {
        public JsTravelEdgeException(String developerMessage = null, Exception innerException = null)
            : base(developerMessage, innerException)
        {

        }
    }
}
