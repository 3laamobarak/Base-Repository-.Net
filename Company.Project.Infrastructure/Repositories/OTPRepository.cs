using Company.Project.Domain.Interfaces;
using Company.Project.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Company.Project.Infrastructure.Repositories
{
    public class OTPRepository : IOTPRepository
    {
        private readonly Context _context;

        public OTPRepository(Context context)
        {
            _context = context;
        }

        public async Task AddAsync(OTP otp)
        {
            await _context.OTPs.AddAsync(otp);
            await _context.SaveChangesAsync();
        }

        public async Task<OTP> GetByEmailAsync(string email)
        {
            return await _context.OTPs
                .OrderByDescending(o => o.ExpirationTime)
                .FirstOrDefaultAsync(o => o.Email == email && !o.IsUsed);
        }

        public async Task UpdateAsync(OTP otp)
        {
            _context.OTPs.Update(otp);
            await _context.SaveChangesAsync();
        }
    }
}
