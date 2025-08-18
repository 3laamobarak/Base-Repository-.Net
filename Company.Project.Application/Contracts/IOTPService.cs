namespace Company.Project.Application.Contracts;

public interface IOTPService
{
    Task<string> GenerateAndSendOTPAsync(string email);
    Task<bool> ValidateOTPAsync(string email, string code);
}