using System.ComponentModel.DataAnnotations;

namespace Company.Project.Application.DTO.Account
{
    public class RegisterModel
    {
        [Required, StringLength(100)]
        public string FirstName { get; set; }

        [Required, StringLength(100)]
        public string LastName { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, StringLength(128)]
        [EmailAddress]
        public string Email { get; set; }
        [Required, StringLength(14)]
        public string NID { get; set; }

        [Required, StringLength(256)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
