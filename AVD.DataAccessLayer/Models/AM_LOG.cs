using System;
using System.Collections.Generic;

namespace AVD.DataAccessLayer.Models
{
    public partial class AM_LOG : BaseModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string ActionName { get; set; }
        public string ActionDescription { get; set; }
        public System.DateTime ActionDate { get; set; }
    }
}
