using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Core.Users.Dtos
{
    public class UserTCAcceptedBodyDto
    {
        public bool TCAccepted { get; set; }
        public String firstName { get; set; }
        public String lastName { get; set; }
    }
}
