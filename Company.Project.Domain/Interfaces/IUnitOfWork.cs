using Company.Project.Domain.Models;

namespace Company.Project.Domain.Interfaces
{
    public interface IUnitOfWork :IDisposable
    {
        IBaseRepository<ExampleClass> ExampleClass { get; }
        IBaseRepository<OTP> OTPs { get; }
        IBaseRepository<ApplicationUser> ApplicationUsers { get; }
        IBaseRepository<ImageFile> ImageFiles { get; }
        // IBaseRepository<ChatBotMessages> ChatBotMessages { get; }
        IChatBotMessageRepository ChatBotMessages { get; }
        IBaseRepository<PaymentTransaction> PaymentTransactions { get; }
        IBaseRepository<PaymentMethod> PaymentMethods { get; }
        Task CompleteAsync();
        void Dispose();
    }
}
