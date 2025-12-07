using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using static Company.Project.Domain.Enums.Enums;

namespace Company.Project.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required , MaxLength(50)]
        public string FirstName { get; set; }
        [Required , MaxLength(50)]
        public string LastName { get; set; }
        public string NID { get; set; }
        public int Age
        {
            get
            {
                if (NID == null)
                {
                    throw new ArgumentNullException(nameof(NID));
                }

                if (NID.Length >= 6 && int.TryParse(NID.Substring(1, 6), out int birthdate))
                {
                    int birthYear = birthdate / 10000;
                    int birthMonth = birthdate / 100 % 100;
                    int birthDay = birthdate % 100;
                    int fullYear;
                    if (birthYear >= 0 && birthYear <= 99)
                    {
                        fullYear = birthYear < 50 ? 2000 + birthYear : 1900 + birthYear;
                    }
                    else
                    {
                        fullYear = birthYear;
                    }

                    int currentYear = DateTime.Now.Year;
                    int calculatedAge = currentYear - fullYear;
                    if (birthMonth > DateTime.Now.Month ||
                        birthMonth == DateTime.Now.Month && birthDay > DateTime.Now.Day)
                    {
                        calculatedAge--;
                    }

                    return calculatedAge;
                }

                return 0;
            }
        }
        public GenderType? Gender
        {
            get
            {
                if (NID == null)
                {
                    throw new ArgumentNullException(nameof(NID));
                }

                char genderchar = NID[12];
                if (char.IsDigit(genderchar))
                {
                    int GenderNumber = int.Parse(genderchar.ToString());
                    return GenderNumber % 2 == 1 ? GenderType.Male : GenderType.Female;
                }

                throw new ArgumentException("Enter the NID first");
            }
        }
        public ICollection<RefreshToken>? RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
