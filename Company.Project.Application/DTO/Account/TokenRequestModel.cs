using System.ComponentModel.DataAnnotations;

namespace Company.Project.Application.DTO.Account
{
    public class TokenRequestModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
