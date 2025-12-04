using EvdeSaglik.Entity.Entities;
using EvdeSaglik.Repositories.Data;
using EvdeSaglik.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvdeSaglik.Repositories.Repositories;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Message>> GetUserMessagesAsync(Guid userId)
    {
        return await _dbSet
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Include(m => m.Service)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetConversationAsync(Guid userId, Guid otherUserId)
    {
        return await _dbSet
            .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                       (m.SenderId == otherUserId && m.ReceiverId == userId))
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Include(m => m.Service)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }
}
