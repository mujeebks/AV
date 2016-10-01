using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AVD.DataAccessLayer;

namespace AVD.DataAccessLayer.Models
{
    public partial class ApiAclEntry : BaseModel
    {
        [Key]
        public int ApiAclEntryId { get; set; }
        public string Path { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}
