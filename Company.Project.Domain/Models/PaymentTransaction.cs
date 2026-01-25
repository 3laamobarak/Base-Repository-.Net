using static Company.Project.Domain.Enums.Enums;

namespace Company.Project.Domain.Models
{
    public class PaymentTransaction : BaseEntity
    {
        public string UserId { get; set; }= string.Empty;
        public ApplicationUser? User { get; set; } 
        
        public string Gateway { get; set; } = string.Empty;
        public string ExternalTransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public TransactionStatus Status { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
        public string? FailureReason { get; set; }
    }
}