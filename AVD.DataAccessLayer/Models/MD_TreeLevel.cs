using System;
using System.Collections.Generic;

namespace AVD.DataAccessLayer.Models
{
    public partial class MD_TreeLevel : BaseModel
    {
        public int ID { get; set; }
        public int Level { get; set; }
        public string LevelName { get; set; }
        public int AttributeID { get; set; }
        public Nullable<bool> IsPercentage { get; set; }
    }
}
