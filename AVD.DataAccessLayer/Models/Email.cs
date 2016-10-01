using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AVD.DataAccessLayer;

namespace AVD.DataAccessLayer.Models
{
    public partial class Email : BaseModel
    {
        [Key]
        public int EmailId { get; set; }
        public int EmailTypeId { get; set; }
        public string ToAddress { get; set; }
        public System.DateTime SendDate { get; set; }
        public string DocumentLink { get; set; }
        public int UserId { get; set; }
        public bool IsSuccess { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public string Url { get; set; }
        public virtual EmailType EmailType { get; set; }
        public virtual User User { get; set; }
    }
}
