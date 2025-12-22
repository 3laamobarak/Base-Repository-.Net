using Company.Project.Domain.Interfaces;
using Company.Project.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Company.Project.Infrastructure.Repositories
{
    public class ChatBotMessageRepository : BaseRepository<ChatBotMessages>, IChatBotMessageRepository
    {
        private readonly Context _context;

        public ChatBotMessageRepository(Context context) : base(context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<ChatBotMessages>> GetUserMessagesAsync(string userId)
        {
            return await _context.ChatBotMessages.Where(m => m.UserId == userId).OrderBy(m => m.CreatedAt).ToListAsync();
        }

    }
}
