using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Company.Project.Domain.Models
{
    public class ChatBotMessages : BaseEntity
    {
        [Required]
        public string Message { get; set; }
        [Required]
        [MaxLength(50)]
        public string Sender { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }
        
    }
}
