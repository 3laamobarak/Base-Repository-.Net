using Company.Project.Application.DTO.PaymentDTO;
using Company.Project.Domain.Models;

namespace Company.Project.Application.Contracts
{
    public interface IStripeService
    {
        Task<PaymentResultDto> ProcessPaymentAsync(CreatePaymentRequest request, string userId);
        Task<PaymentResultDto> ProcessSavedPaymentAsync(int savedPaymentMethodId , decimal amount, string currency, string userId);
        Task<SavePaymentMethodResultDto> SavePaymentMethodAsync(SavePaymentMethodRequest request,  string userId);
        Task<bool> RefundPaymentAsync(string externalTransactionId, decimal? amount = null);
        Task<List<PaymentMethodDto>> GetPaymentMethods(string userId);
        Task<List<PaymentTransactionDto>> GetTransactions(string userId);
        
    }
}
