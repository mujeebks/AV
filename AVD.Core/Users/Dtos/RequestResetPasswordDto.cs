using System.ComponentModel.DataAnnotations;

namespace AVD.Core.Users.Dtos
{
    public class RequestResetPasswordDto
    {
        [Required]
        public string Email { get; set; }
    }
}