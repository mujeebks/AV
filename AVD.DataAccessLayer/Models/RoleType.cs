using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Models;

namespace AVD.DataAccessLayer.Models
{
    public partial class RoleType : BaseModel
    {
        public RoleType()
        {
            this.Roles = new List<Role>();
        }

        [Key]
        public int RoleTypeId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}
