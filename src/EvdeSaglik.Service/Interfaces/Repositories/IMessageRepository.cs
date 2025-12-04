using EvdeSaglik.Entity.Entities;

namespace EvdeSaglik.Service.Interfaces.Repositories;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<IEnumerable<Message>> GetUserMessagesAsync(Guid userId);
    Task<IEnumerable<Message>> GetConversationAsync(Guid userId, Guid otherUserId);
}
