namespace Company.Project.Application.DTO.PaymentDTO
{
    public class CreatePaymentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string Gateway { get; set; } = "paymob";
        public string Description { get; set; } = "";
        public bool SavePaymentMethod { get; set; } 
        public int? SavePaymentMethodId { get; set; }
        public string? cardToken { get; set; } 
    }
}
