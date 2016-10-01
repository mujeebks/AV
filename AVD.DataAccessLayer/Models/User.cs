using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AVD.DataAccessLayer;

namespace AVD.DataAccessLayer.Models
{
    public partial class User : BaseModel
    {
        public override string ToString()
        {
            return "P:" + this.UserId + ", " + this.LastName + "/" + this.FirstName + "/" + this.MiddleName;
        }

        public User()
        {
            this.CachedItems = new List<CachedItem>();
            this.Emails = new List<Email>();
        }

        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public bool Isactive { get; set; }
        public bool password_reset { get; set; }
        private DateTime? lastLoginDateTime { get; set; }
        public DateTime? last_login { get; set; }
        public DateTime? LastLoginDateTime { get { return lastLoginDateTime;} set { if(value != null) { lastLoginDateTime = DateTime.SpecifyKind(value.Value, DateTimeKind.Local); } else {  lastLoginDateTime = value; } } }
        public virtual ICollection<Email> Emails { get; set; }
        public virtual ICollection<CachedItem> CachedItems { get; set; }
    }
}
