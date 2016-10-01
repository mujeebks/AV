using AVD.Common.Primitives;

namespace AVD.Core.Users.Dtos
{
    public class ResetPasswordDto
    {
        public string Token { get; set; }
        public AVDPassword NewPassword { get; set; }
    }
}
