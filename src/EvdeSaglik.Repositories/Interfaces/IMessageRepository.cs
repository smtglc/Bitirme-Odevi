using EvdeSaglik.Entity.Entities;

namespace EvdeSaglik.Repositories.Interfaces;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<IEnumerable<Message>> GetUserMessagesAsync(Guid userId);
    Task<IEnumerable<Message>> GetConversationAsync(Guid userId, Guid otherUserId);
}
