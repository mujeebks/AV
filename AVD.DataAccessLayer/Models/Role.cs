using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Models;

namespace AVD.DataAccessLayer.Models
{
    public partial class Role : BaseModel
    {
        public Role()
        {
            this.ApiAclEntries = new List<ApiAclEntry>();
        }

        [Key]
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string ADRoleName { get; set; }
        public int? RoleTypeId { get; set; }
        public virtual ICollection<ApiAclEntry> ApiAclEntries { get; set; }
        public virtual RoleType RoleType { get; set; }
    }
}
