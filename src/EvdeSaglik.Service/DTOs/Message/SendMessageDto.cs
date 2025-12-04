namespace EvdeSaglik.Service.DTOs.Message;

public class SendMessageDto
{
    public Guid ReceiverId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ServiceId { get; set; }
}
