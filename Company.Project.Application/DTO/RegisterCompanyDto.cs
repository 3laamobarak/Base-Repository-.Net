namespace Company.Project.Application.DTO;

public class RegisterCompanyDto
{
    public string EnglishName { get; set; }
    public string ArabicName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? Phone { get; set; }
    public string? Logo { get; set; }
    public string WebsiteURL { get; set; }
    public string otpCode { get; set; }
}
