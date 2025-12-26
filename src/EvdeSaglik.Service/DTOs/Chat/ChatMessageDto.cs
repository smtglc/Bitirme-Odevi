namespace EvdeSaglik.Service.DTOs.Chat;

public class ChatMessageDto
{
    public string Role { get; set; } = string.Empty; // "user" or "model"
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
