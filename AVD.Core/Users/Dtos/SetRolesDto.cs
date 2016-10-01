using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVD.DataAccessLayer.Enums;

namespace AVD.Core.Users.Dtos
{
    public class SetRolesDto
    {
        public List<RoleTypes> Roles { get; set; } 
    }
}
