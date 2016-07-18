using System;
using System.Collections.Generic;

namespace AVD.DataAccessLayer.Models
{
    public partial class MD_Feature : BaseModel
    {
        public int ID { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public Nullable<int> ModuleID { get; set; }
        public Nullable<bool> IsEnable { get; set; }
        public Nullable<bool> IsTopNavigation { get; set; }
    }
}
