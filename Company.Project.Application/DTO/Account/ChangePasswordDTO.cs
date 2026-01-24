using System.ComponentModel.DataAnnotations;

namespace Company.Project.Application.DTO
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm password is required.")]
        // should be equal to NewPassword
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
