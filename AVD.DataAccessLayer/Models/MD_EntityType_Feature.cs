using System;
using System.Collections.Generic;

namespace AVD.DataAccessLayer.Models
{
    public partial class MD_EntityType_Feature : BaseModel
    {
        public int TypeID { get; set; }
        public int FeatureID { get; set; }
        public int ID { get; set; }
        public virtual MD_EntityType MD_EntityType { get; set; }
    }
}
