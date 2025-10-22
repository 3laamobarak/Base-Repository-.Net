using Company.Project.Domain.Interfaces;
using Company.Project.Domain.Models;
using Company.Project.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Company.Project.Infrastructure.UnitOfWork
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly Context _context;
        public IBaseRepository<ExampleClass> ExampleClass { get; private set; }
        public IBaseRepository<OTP> OTPs { get; private set; }
        public IBaseRepository<ApplicationUser> ApplicationUsers { get; private set; }
        
        public UnitOfWork(Context context )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            ExampleClass = new BaseRepository<ExampleClass>(_context);
            OTPs = new BaseRepository<OTP>(_context);
            ApplicationUsers = new BaseRepository<Domain.Models.ApplicationUser>(_context);
        }

        public async Task Completeasync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}