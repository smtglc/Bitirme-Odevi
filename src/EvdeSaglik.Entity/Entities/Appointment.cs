using EvdeSaglik.Entity.Common;
using EvdeSaglik.Entity.Enums;

namespace EvdeSaglik.Entity.Entities;

public class Appointment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ServiceId { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public int DurationMinutes { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string? PatientNotes { get; set; }
    public string? DoctorNotes { get; set; }
    public decimal TotalAmount { get; set; }

  
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public Payment? Payment { get; set; }
}
