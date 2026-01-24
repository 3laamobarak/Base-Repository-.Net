using System.ComponentModel.DataAnnotations;

namespace Company.Project.Application.DTO.PaymentDTO
{
    public class CreatePaymentRequest
    {
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Currency { get; set; } = "USD";
        [Required]
        public string Gateway { get; set; } = "stripe";
        public string Description { get; set; }
        public bool SavePaymentMethod { get; set; } 
        public int? SavePaymentMethodId { get; set; }
        public string? cardToken { get; set; } 
    }
}
