using EvdeSaglik.Service.DTOs.Message;

namespace EvdeSaglik.Service.Interfaces;

public interface IMessageService
{
    Task<List<MessageDto>> GetUserMessagesAsync(Guid userId);
    Task<List<MessageDto>> GetConversationAsync(Guid userId, Guid otherUserId);
    Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageDto dto);
    Task MarkAsReadAsync(Guid messageId);
}
