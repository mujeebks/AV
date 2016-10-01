using System;

namespace AVD.Core.Users
{
    public class ResetUserPasswordToken
    {
        public int UserId { get; set; }
        public string ResetToken { get; set; }
        public DateTime ResetTokenDateTime { get; set; }
    }
}