using Company.Project.Domain.Models;

namespace Company.Project.Application.DTO.PaymentDTO
{
    public class SavePaymentMethodResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = "";
        public PaymentMethod? PaymentMethod { get; set; }
        
    }
}
