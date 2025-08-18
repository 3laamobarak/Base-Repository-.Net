using System.ComponentModel.DataAnnotations;

namespace Company.Project.Application.DTO
{
    public class ResetPasswordDTO
    {
        [Required (ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        
        [Required]
        public string Token { get; set; }
        
        [Required (ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; }
        
    }
}
