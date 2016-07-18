using System;
using System.Collections.Generic;

namespace AVD.DataAccessLayer.Models
{
    public partial class UM_User : BaseModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public byte[] Password { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public Nullable<bool> Gender { get; set; }
        public Nullable<bool> Isactive { get; set; }
        public Nullable<bool> password_reset { get; set; }
        public Nullable<System.DateTime> last_login { get; set; }
    }
}
