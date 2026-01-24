using Company.Project.Domain.Models;

namespace Company.Project.Application.DTO.PaymentDTO
{
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public PaymentTransaction Transaction { get; set; }
    }
}
