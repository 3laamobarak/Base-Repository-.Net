using Company.Project.Application.Contracts;
using Company.Project.Application.DTO.PaymentDTO;
using Company.Project.Domain.Enums;
using Company.Project.Domain.Interfaces;
using Company.Project.Domain.Models;
using Company.Project.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Stripe;
namespace Company.Project.Application.Services
{
    public class StripeService : IStripeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration _config;

        public StripeService(IUnitOfWork _unitofwork, IConfiguration config)
        {
            unitOfWork = _unitofwork;
            _config = config;
//            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }
        public async Task<PaymentResultDto> ProcessPaymentAsync(CreatePaymentRequest request, string userId)
        {
            try
            {
                var user = await unitOfWork.ApplicationUsers.GetByIdAsync(userId);
                if (user == null)
                {
                    return new PaymentResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "User not found"
                    };
                }

                var customerService = new CustomerService();
                var customer = await GetOrCreateStripeCustomer(user, customerService);
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(request.Amount * 100),
                    Currency = request.Currency.ToLower(),
                    Customer = customer.Id,
                    PaymentMethod = request.cardToken,
                    PaymentMethodTypes = new List<string>{"card"},
                    Description = request.Description,
                    ConfirmationMethod = "manual",
                    Confirm = true,
                    ReturnUrl = _config["FrontendUrl"] + "/payment/return"
                };
                var service = new PaymentIntentService();
                var paymentintent = await service.CreateAsync(options);

                var transaction = new PaymentTransaction
                {
                    UserId = userId,
                    Gateway = "Stripe",
                    ExternalTransactionId = paymentintent.Id,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    Status = MapStripeStatus(paymentintent.Status),
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow
                };
                await unitOfWork.PaymentTransactions.AddAsync(transaction);

                if (request.SavePaymentMethod && paymentintent.PaymentMethod != null)
                {
                    await SaveStripePaymentMethod(paymentintent.PaymentMethod.Id, userId, customer.Id);
                }

                await unitOfWork.CompleteAsync();
                return new PaymentResultDto
                {
                    IsSuccess = paymentintent.Status == "succeeded",
                    TransactionId = paymentintent.Id,
                    Status = paymentintent.Status,
                    ClientSecret = paymentintent.ClientSecret,
                    Transaction = new PaymentTransactionDto
                    {
                        Id = transaction.Id,
                        Amount = transaction.Amount,
                        Currency = transaction.Currency,
                        Status = transaction.Status.ToString(),
                        Description = transaction.Description,
                        CompletedAt = transaction.CompletedAt,
                        Gateway = transaction.Gateway,
                        ExternalTransactionId = transaction.ExternalTransactionId,
                        CreatedAt = transaction.CreatedAt
                    }
                };
            }
            catch (StripeException ex)
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = ex.StripeError.Message
                };
            }
            catch (Exception ex)
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<PaymentResultDto> ProcessSavedPaymentAsync(int savedPaymentMethodId, decimal amount, string currency, string userId)
        {
            var savedMethod = await unitOfWork.PaymentMethods.GetByIdAsync(savedPaymentMethodId);
            if (savedMethod == null || savedMethod.UserId != userId)
                return new PaymentResultDto { IsSuccess = false, ErrorMessage = "Payment method not found or not owned by user" };
            var request = new CreatePaymentRequest
            {
                Amount = amount,
                Currency = currency,
                Gateway = "stripe",
                Description = "Payment using saved method",
                cardToken = savedMethod.TokenId,
                SavePaymentMethod = false
            };
            return await ProcessPaymentAsync(request, userId);
        }

        public async Task<SavePaymentMethodResultDto> SavePaymentMethodAsync(SavePaymentMethodRequest request, string userId)
        {
            try
            {
                var user = await unitOfWork.ApplicationUsers.GetByIdAsync(userId);
                if (user == null)
                    return new SavePaymentMethodResultDto { IsSuccess = false, ErrorMessage = "User not found" };

                var customerService = new CustomerService();
                var customer = await GetOrCreateStripeCustomer(user, customerService);

                var attachOptions = new PaymentMethodAttachOptions { Customer = customer.Id };
                var paymentMethodService = new PaymentMethodService();
                var paymentMethod = await paymentMethodService.AttachAsync(request.CardToken, attachOptions);

                var savedMethod = new Domain.Models.PaymentMethod
                {
                    UserId = userId,
                    Gateway = "stripe",
                    ExternalId = customer.Id,
                    TokenId = paymentMethod.Id,
                    LastFourDigits = paymentMethod.Card.Last4,
                    CardBrand = paymentMethod.Card.Brand,
                    ExpiryMonth = (int)paymentMethod.Card.ExpMonth,
                    ExpiryYear = (int)paymentMethod.Card.ExpYear,
                    CreatedAt = DateTime.UtcNow
                };

                await unitOfWork.PaymentMethods.AddAsync(savedMethod);

                if (request.SetAsDefault)
                {
                    var allMethods = await unitOfWork.PaymentMethods.GetByExpressionAsync(m => m.UserId == userId);
                    foreach (var m in allMethods)
                        m.IsDefault = m.Id == savedMethod.Id;

                    foreach (var m in allMethods)
                        await unitOfWork.PaymentMethods.UpdateAsync(m);
                }

                await unitOfWork.CompleteAsync();

                return new SavePaymentMethodResultDto
                {
                    IsSuccess = true,
                    PaymentMethod = new PaymentMethodDto
                    {
                        CreatedAt = savedMethod.CreatedAt,
                        UpdatedAt = savedMethod.UpdatedAt,
                        IsDeleted = savedMethod.IsDeleted,
                        UserId = savedMethod.UserId,
                        Gateway = savedMethod.Gateway,
                        ExternalId = savedMethod.ExternalId,
                        TokenId = savedMethod.TokenId,
                        Last4 = savedMethod.LastFourDigits,
                        CardBrand = savedMethod.CardBrand,
                        ExpMonth = savedMethod.ExpiryMonth,
                        ExpYear = savedMethod.ExpiryYear,
                        IsDefault = savedMethod.IsDefault
                    }
                };
            }
            catch (StripeException ex)
            {
                return new SavePaymentMethodResultDto { IsSuccess = false, ErrorMessage = ex.StripeError?.Message ?? ex.Message };
            }
            catch (Exception ex)
            {
                return new SavePaymentMethodResultDto { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
        
        public async Task<bool> RefundPaymentAsync(string TransactionId, decimal? amount = null)
        {
            try
            {
                var tx = await unitOfWork.PaymentTransactions.GetByExpressionSingleAsync(t => t.ExternalTransactionId == TransactionId);
                if (tx == null|| tx.Status!= Enums.TransactionStatus.Success)
                    return false;

                var refundOptions = new RefundCreateOptions
                {
                    PaymentIntent = TransactionId
                };
                if(amount.HasValue)
                    refundOptions.Amount = (long)(amount.Value * 100);
                var refundService = new RefundService();
                var refund = await refundService.CreateAsync(refundOptions);
                
                if(refund.Status == "succeeded" || refund.Status== "pending")
                {
                    tx.Status = Enums.TransactionStatus.Refunded;
                    tx.CompletedAt = DateTime.UtcNow;
                    await unitOfWork.PaymentTransactions.UpdateAsync(tx);
                    await unitOfWork.CompleteAsync();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private async Task<Customer> GetOrCreateStripeCustomer(ApplicationUser user, CustomerService customerService)
        {
            var existingPaymentMethod = await unitOfWork.PaymentMethods
                .GetByExpressionSingleAsync(pm => pm.UserId == user.Id && pm.Gateway == "Stripe");
            if (existingPaymentMethod != null)
            {
                return await customerService.GetAsync(existingPaymentMethod.ExternalId);
            }
            var customerOptions = new CustomerCreateOptions
            {
                Email = user.Email,
                Name = user.UserName
            };
            return await customerService.CreateAsync(customerOptions);
        }
        private async Task<Domain.Models.PaymentMethod> SaveStripePaymentMethod(string paymentMethodId, string userId, string customerId)
        {
            var paymentMethodService = new PaymentMethodService();
            var stripePaymentMethod = await paymentMethodService.GetAsync(paymentMethodId);
            var saveMethod = new Domain.Models.PaymentMethod
            {
                UserId = userId,
                Gateway = "Stripe",
                ExternalId = customerId,
                TokenId = paymentMethodId,
                LastFourDigits = stripePaymentMethod.Card.Last4,
                CardBrand = stripePaymentMethod.Card.Brand,
                ExpiryMonth = (int)stripePaymentMethod.Card.ExpMonth,
                ExpiryYear = (int)stripePaymentMethod.Card.ExpYear,
                CreatedAt = DateTime.UtcNow
            };
            await unitOfWork.PaymentMethods.AddAsync(saveMethod);
            await unitOfWork.CompleteAsync();
            return saveMethod;
        }
        private Enums.TransactionStatus MapStripeStatus(string stripeStatus) => stripeStatus switch
        {
            "succeeded" => Enums.TransactionStatus.Success,
            "requires_payment_method" => Enums.TransactionStatus.Failed,
            "requires_confirmation" => Enums.TransactionStatus.Pending,
            "requires_action" => Enums.TransactionStatus.Pending,
            "processing" => Enums.TransactionStatus.Pending,
            _ => Enums.TransactionStatus.Failed,
        };
        public async Task<List<PaymentMethodDto>> GetPaymentMethods(string userId)
        {
            var methods = await unitOfWork.PaymentMethods
                .GetByExpressionAsync(pm => pm.UserId == userId);
            return methods.Select(m => new PaymentMethodDto
            {
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt,
                IsDeleted = m.IsDeleted,
                UserId = m.UserId,
                Gateway = m.Gateway,
                ExternalId = m.ExternalId,
                TokenId = m.TokenId,
                Last4 = m.LastFourDigits,
                CardBrand = m.CardBrand,
                ExpMonth = m.ExpiryMonth,
                ExpYear = m.ExpiryYear,
                IsDefault = m.IsDefault
            }).ToList();

        }
        public async Task<List<PaymentTransactionDto>> GetTransactions(string userId)
        {
            var transactions = await unitOfWork.PaymentTransactions
                .GetByExpressionAsync(tx => tx.UserId == userId);
            return transactions.Select(t => new PaymentTransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Currency = t.Currency,
                Status = t.Status.ToString(),
                Description = t.Description,
                CompletedAt = t.CompletedAt,
                Gateway = t.Gateway,
                ExternalTransactionId = t.ExternalTransactionId,
                CreatedAt = t.CreatedAt
            }).ToList();

        }

    }
}
