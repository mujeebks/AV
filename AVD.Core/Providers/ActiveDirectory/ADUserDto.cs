using System;
using System.Linq;
using System.Text;

namespace AVD.Core.Providers.ActiveDirectory
{
    public class ADUserDto
    {
        public string SAMAccountName { get; set; }
        public string Mail { get; set; }
        public string DisplayName { get; set; }

        override public string ToString()
        {
            return "DisplayName=" + DisplayName + ", SAMAccountName= " + SAMAccountName + ", Mail=" + Mail;
        }
    }
}
