using System;
using System.Collections.Generic;

namespace AVD.DataAccessLayer.Models
{
    public partial class Error : BaseModel
    {
        public int ID { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public System.DateTime DateCreated { get; set; }
    }
}
