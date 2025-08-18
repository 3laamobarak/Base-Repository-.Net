using Company.Project.Application.DTO;

namespace Company.Project.Application.Contracts;

public interface IEmailService
{
    public Task SendEmailAsync(EmailDTO emailDto);

}