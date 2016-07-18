using System;
using System.Collections.Generic;

namespace AVD.DataAccessLayer.Models
{
    public partial class MD_EntityType : BaseModel
    {
        public MD_EntityType()
        {
            this.MD_EntityType_Feature = new List<MD_EntityType_Feature>();
        }

        public int ID { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public Nullable<int> ModuleID { get; set; }
        public Nullable<int> Category { get; set; }
        public string ShortDescription { get; set; }
        public string ColorCode { get; set; }
        public Nullable<bool> IsAssociate { get; set; }
        public Nullable<bool> IsRootLevel { get; set; }
        public virtual ICollection<MD_EntityType_Feature> MD_EntityType_Feature { get; set; }
    }
}
