using EvdeSaglik.Entity.Common;

namespace EvdeSaglik.Entity.Entities;

public class Message : BaseEntity
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid? ServiceId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }


    public User Sender { get; set; } = null!;
    public User Receiver { get; set; } = null!;
    public Service? Service { get; set; }
}
