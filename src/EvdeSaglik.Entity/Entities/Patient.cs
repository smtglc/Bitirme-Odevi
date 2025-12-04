using EvdeSaglik.Entity.Common;

namespace EvdeSaglik.Entity.Entities;

public class Patient : BaseEntity
{
    public Guid UserId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? MedicalHistory { get; set; }
    public string? EmergencyContact { get; set; }

    
    public User User { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
