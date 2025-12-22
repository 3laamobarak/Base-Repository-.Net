using Company.Project.Domain.Models;

namespace Company.Project.Domain.Interfaces
{
    public interface IChatBotMessageRepository : IBaseRepository<ChatBotMessages>
    {
        Task<IEnumerable<ChatBotMessages>> GetUserMessagesAsync(string userId);
        
    }
}
