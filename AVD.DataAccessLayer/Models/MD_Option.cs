using System;
using System.Collections.Generic;

namespace AVD.DataAccessLayer.Models
{
    public partial class MD_Option : BaseModel
    {
        public int ID { get; set; }
        public string Caption { get; set; }
        public Nullable<int> AttributeID { get; set; }
        public int SortOrder { get; set; }
    }
}
