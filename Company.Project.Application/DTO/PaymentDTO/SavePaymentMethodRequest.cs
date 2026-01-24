using System.ComponentModel.DataAnnotations;

namespace Company.Project.Application.DTO.PaymentDTO
{
    public class SavePaymentMethodRequest
    {
        [Required]
        public string Gateway { get; set; } 
        [Required]
        public string CardToken { get; set; }
        public bool SetAsDefault { get; set;  }
    }
}
