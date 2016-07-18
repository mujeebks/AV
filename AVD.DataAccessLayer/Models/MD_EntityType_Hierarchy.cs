using System;
using System.Collections.Generic;

namespace AVD.DataAccessLayer.Models
{
    public partial class MD_EntityType_Hierarchy : BaseModel
    {
        public int ID { get; set; }
        public int ParentEntityTypeID { get; set; }
        public int ChildEntityTypeID { get; set; }
        public int SortOrder { get; set; }
    }
}
