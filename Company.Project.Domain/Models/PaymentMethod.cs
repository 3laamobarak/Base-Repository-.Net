namespace Company.Project.Domain.Models
{
    public class PaymentMethod :BaseEntity
    {
        public string UserId { get; set; }= string.Empty;
        public ApplicationUser User { get; set; } = null!;
        
        public string Gateway { get; set; } = string.Empty;
        public string ExternalId { get; set; } = string.Empty;
        public string TokenId { get; set; } = string.Empty;
        public string LastFourDigits { get; set; } = string.Empty;
        public string CardBrand { get; set; } = string.Empty;
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public bool IsDefault { get; set; }
    }
}
