using Company.Project.Domain.Models;

namespace Company.Project.Application.DTO.PaymentDTO
{
    public class SavePaymentMethodResultDto
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }= "";
        public PaymentMethodDto? PaymentMethod { get; set; }
        
    }
}
