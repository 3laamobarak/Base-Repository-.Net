using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Project.Application.DTO.PaymentDTO
{
    public class PaymentResultDto
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; } = "";
        public string Status { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        public string ClientSecret { get; set; } = "";  // ← NEW FIELD
        public PaymentTransactionDto? Transaction { get; set; }
    }
}
