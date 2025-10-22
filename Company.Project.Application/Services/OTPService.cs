using Company.Project.Application.Contracts;
using Company.Project.Application.DTO;
using Company.Project.Domain.Interfaces;
using Company.Project.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Company.Project.Application.Services
{
    public class OTPService : IOTPService
    {
        private readonly IOTPRepository _otpRepository;
        private readonly IEmailService _emailService;

        public OTPService(
            IOTPRepository otpRepository,
            IEmailService emailService)
        {
            _otpRepository = otpRepository;
            _emailService = emailService;
        }

        public async Task<string> GenerateAndSendOTPAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email address is required");
            }

            var otp = new Random().Next(100000, 999999).ToString();
            var otpEntity = new OTP
            {
                Code = otp,
                Email = email, // Add Email field to OTP model
                ExpirationTime = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            await _otpRepository.AddAsync(otpEntity);

            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Email Verification",
                Body =
                    $"<h1>Email Verification Code</h1><p>Your verification code is: <strong>{otp}</strong></p><p>This code will expire in 5 minutes.</p>"
            };

            await _emailService.SendEmailAsync(emailDto);
            return otp;
        }

        public async Task<bool> ValidateOTPAsync(string email, string otp)
        {
            var otpEntity = await _otpRepository.GetByEmailAsync(email);
            if (otpEntity == null || otpEntity.IsUsed || otpEntity.ExpirationTime < DateTime.UtcNow)
            {
                return false; // OTP not found, used, or expired
            }

            if (otpEntity.Code != otp)
            {
                return false; // OTP does not match
            }

            otpEntity.IsUsed = true;
            await _otpRepository.UpdateAsync(otpEntity);
            return true;
        }
    }
}
