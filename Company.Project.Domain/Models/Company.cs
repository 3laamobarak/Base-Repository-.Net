// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Identity;
//
// namespace Company.Project.Domain.Models
// {
//     public class Company : IdentityUser
//     {
//         [Required]
//         public string EnglishName { get; set; }
//         [Required]
//         public string ArabicName { get; set; }
//         [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone number must start with 010, 011, 012, or 015 and be 11 digits long.")]
//         public string? Phone { get; set; }
//         public string? Logo { get; set; }
//         public string websiteURL { get; set; }
//     }
// }
