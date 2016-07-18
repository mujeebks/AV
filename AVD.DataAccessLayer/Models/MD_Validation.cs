using System;
using System.Collections.Generic;

namespace AVD.DataAccessLayer.Models
{
    public partial class MD_Validation : BaseModel
    {
        public int ID { get; set; }
        public Nullable<int> EntityTypeID { get; set; }
        public Nullable<int> RelationShipID { get; set; }
        public string Name { get; set; }
        public string ValueType { get; set; }
        public string Value { get; set; }
        public string ErrorMessage { get; set; }
    }
}
