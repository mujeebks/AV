using System;
using System.Collections.Generic;

namespace AVD.DataAccessLayer.Models
{
    public partial class MD_AttributeType : BaseModel
    {
        public MD_AttributeType()
        {
            this.MD_Attribute = new List<MD_Attribute>();
        }

        public int ID { get; set; }
        public string Caption { get; set; }
        public Nullable<bool> IsSelectable { get; set; }
        public string DataType { get; set; }
        public string SqlType { get; set; }
        public Nullable<int> Length { get; set; }
        public Nullable<bool> IsNullable { get; set; }
        public virtual ICollection<MD_Attribute> MD_Attribute { get; set; }
    }
}
