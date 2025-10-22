using Company.Project.Application.DTO.Account;

namespace Company.Project.Application.Contracts
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);
        Task<string> AddRoleAsync(AddRole model);
        Task<AuthModel> RefreshTokenAsync(string token);
    }
}
