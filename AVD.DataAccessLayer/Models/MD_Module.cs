using System;
using System.Collections.Generic;

namespace AVD.DataAccessLayer.Models
{
    public partial class MD_Module : BaseModel
    {
        public int ID { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public bool IsEnable { get; set; }
    }
}
