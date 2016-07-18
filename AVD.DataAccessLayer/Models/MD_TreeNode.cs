using System;
using System.Collections.Generic;

namespace AVD.DataAccessLayer.Models
{
    public partial class MD_TreeNode : BaseModel
    {
        public int ID { get; set; }
        public int NodeID { get; set; }
        public int ParentNodeID { get; set; }
        public int Level { get; set; }
        public string KEY { get; set; }
        public int AttributeID { get; set; }
        public string Caption { get; set; }
        public Nullable<int> SortOrder { get; set; }
    }
}
