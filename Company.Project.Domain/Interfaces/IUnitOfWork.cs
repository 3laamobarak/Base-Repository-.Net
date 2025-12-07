using Company.Project.Domain.Models;

namespace Company.Project.Domain.Interfaces
{
    public interface IUnitOfWork :IDisposable
    {
        IBaseRepository<ExampleClass> ExampleClass { get; }
        IBaseRepository<OTP> OTPs { get; }
        IBaseRepository<ApplicationUser> ApplicationUsers { get; }
        IBaseRepository<ImageFile> ImageFiles { get; }
        Task CompleteAsync();
        void Dispose();
    }
}
