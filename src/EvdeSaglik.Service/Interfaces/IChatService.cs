using EvdeSaglik.Service.DTOs.Chat;

namespace EvdeSaglik.Service.Interfaces;

public interface IChatService
{
    Task<ChatResponseDto> GetChatResponseAsync(Guid userId, ChatRequestDto request);
}
