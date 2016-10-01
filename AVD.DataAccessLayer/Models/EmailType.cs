using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TE.DataAccessLayer;

namespace AVD.DataAccessLayer.Models
{
    public partial class EmailType : BaseModel
    {
        public EmailType()
        {
            this.Emails = new List<Email>();
        }

        [Key]
        public int EmailTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Email> Emails { get; set; }
    }
}
