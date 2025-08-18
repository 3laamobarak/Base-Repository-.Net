using System.Net;
using System.Net.Mail;
using Company.Project.Application.Contracts;
using Company.Project.Application.DTO;
using Microsoft.Extensions.Configuration;

namespace Company.Project.Application.Services
{
    public class EmailService :IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(EmailDTO emailDto)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(
                    _configuration["EmailSettings:Email"],
                    _configuration["EmailSettings:Password"]),
                EnableSsl = true
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:Email"]),
                Subject = emailDto.Subject,
                Body = emailDto.Body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(emailDto.To);
            await smtpClient.SendMailAsync(mailMessage);
        }

    }
}
