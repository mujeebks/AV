using System;
using System.Collections.Generic;

namespace AVD.DataAccessLayer.Models
{
    public partial class MD_Attribute : BaseModel
    {
        public int ID { get; set; }
        public string Caption { get; set; }
        public int AttributeTypeID { get; set; }
        public bool IsSystemDefined { get; set; }
        public Nullable<bool> IsSpecial { get; set; }
        public string Description { get; set; }
        public virtual MD_Attribute MD_Attribute1 { get; set; }
        public virtual MD_Attribute MD_Attribute2 { get; set; }
        public virtual MD_AttributeType MD_AttributeType { get; set; }
    }
}
