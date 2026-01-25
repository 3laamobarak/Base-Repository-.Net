using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Project.Application.DTO.PaymentDTO
{
    public class PaymentTransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "";
        public string Status { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? CompletedAt { get; set; }
        public string Gateway { get; set; } = "";
        public string ExternalTransactionId { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string? FailerReason { get; set; }
    }
}
