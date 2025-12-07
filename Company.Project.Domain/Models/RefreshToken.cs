using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Project.Domain.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresOn { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public bool IsRevoked => RevokedOn != null;
        public bool IsActive => RevokedOn == null && !IsExpired;
        public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;
        public string? ReplacedByToken { get; set; }
    }
}
