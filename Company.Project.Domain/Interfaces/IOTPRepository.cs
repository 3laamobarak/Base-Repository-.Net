using Company.Project.Domain.Models;

namespace Company.Project.Domain.Interfaces
{
    public interface IOTPRepository
    {
        Task AddAsync(OTP otp);
        Task<OTP> GetByEmailAsync(string email);
        Task UpdateAsync(OTP otp);
    }
}
