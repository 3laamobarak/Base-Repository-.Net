using System.ComponentModel.DataAnnotations;

namespace Company.Project.Application.DTO.Account
{ 
    public class AddRole
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
