using Company.Project.Application.DTO.PaymentDTO;

namespace Company.Project.Application.Contracts
{
    public interface IPaymentGatewayService
    {
        Task<PaymentResult> ProcessPaymentAsync(CreatePaymentRequest request, string userId);
        Task<PaymentResult> ProcessSavedPaymentAsync(int saveMethodId , decimal amount, string currency, string userId);
        Task<SavePaymentMethodResult> SavePaymentMethodAsync(SavePaymentMethodRequest request,  string userId);
        Task<bool> RefundPaymentAsync(string externalTransactionId, decimal? amount = null);
    }
}
