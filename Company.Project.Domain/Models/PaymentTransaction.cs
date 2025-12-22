using static Company.Project.Domain.Enums.Enums;

namespace Company.Project.Domain.Models
{
    public class PaymentTransaction : BaseEntity
    {
        public string UserId { get; set; }= string.Empty;
        public ApplicationUser User { get; set; } = null;
        
        public string Gateway { get; set; } = string.Empty;
        public string ExternalTransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
        public string Description { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
        public string? FailureReason { get; set; }
    }
}