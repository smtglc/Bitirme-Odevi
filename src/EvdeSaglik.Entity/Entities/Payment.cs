using EvdeSaglik.Entity.Common;
using EvdeSaglik.Entity.Enums;

namespace EvdeSaglik.Entity.Entities;

public class Payment : BaseEntity
{
    public Guid AppointmentId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }

 
    public Appointment Appointment { get; set; } = null!;
}
