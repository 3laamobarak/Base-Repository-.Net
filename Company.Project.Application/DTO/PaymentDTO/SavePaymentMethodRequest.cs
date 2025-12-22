namespace Company.Project.Application.DTO.PaymentDTO
{
    public class SavePaymentMethodRequest
    {
        public string Gateway { get; set; } = "paymob";
        public string CardToken { get; set; } = "";
        public bool SetAsDefault { get; set;  }
    }
}
