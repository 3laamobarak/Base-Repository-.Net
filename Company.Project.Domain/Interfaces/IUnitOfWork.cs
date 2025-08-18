using Company.Project.Domain.Models;

namespace Company.Project.Domain.Interfaces
{
    public interface IUnitOfWork :IDisposable
    {
        //IExampleClassRepository ExampleClassRepository { get; }
        IBaseRepository<OTP> OTPs { get; }
        Task Completeasync();
        void Dispose();
    }
}
