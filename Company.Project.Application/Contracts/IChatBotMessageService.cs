using System.Collections;
using Company.Project.Domain.Models;

namespace Company.Project.Application.Contracts
{
    public interface IChatBotMessageService
    {
        Task<(ChatBotMessages userMsg, ChatBotMessages botMsg)> SendMessageAsync(string userId, string message);
        Task<IEnumerable<ChatBotMessages>> GetMessagesByUserAsync(string userId);
        Task<IEnumerable<ChatBotMessages>> GetAllMessagesAsync();
        Task DeleteMessagesByUserAsync(string userId);
    }
}