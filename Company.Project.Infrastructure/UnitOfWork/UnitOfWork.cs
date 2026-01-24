using Company.Project.Domain.Interfaces;
using Company.Project.Domain.Models;
using Company.Project.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Company.Project.Infrastructure.UnitOfWork
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly Context _context;
        public IBaseRepository<T>GetRepository<T>() where T : class
        {
            return new BaseRepository<T>(_context);
        }
        public IBaseRepository<ExampleClass> ExampleClass { get; private set; }
        public IBaseRepository<OTP> OTPs { get; private set; }
        public IBaseRepository<ApplicationUser> ApplicationUsers { get; private set; }
        public IBaseRepository<ImageFile> ImageFiles { get;private set; }
        // public IBaseRepository<ChatBotMessages> ChatBotMessages { get; private set; }
        public IChatBotMessageRepository ChatBotMessages { get; private set; }
        public IBaseRepository<PaymentTransaction> PaymentTransactions { get; private set; }
        public IBaseRepository<PaymentMethod> PaymentMethods { get; private set; }
        public UnitOfWork(Context context )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            ExampleClass = new BaseRepository<ExampleClass>(_context);
            OTPs = new BaseRepository<OTP>(_context);
            ApplicationUsers = new BaseRepository<ApplicationUser>(_context);
            ImageFiles = new BaseRepository<ImageFile>(_context);
            ChatBotMessages = new ChatBotMessageRepository(_context);
            PaymentTransactions = new BaseRepository<PaymentTransaction>(_context);
            PaymentMethods = new BaseRepository<PaymentMethod>(_context);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}