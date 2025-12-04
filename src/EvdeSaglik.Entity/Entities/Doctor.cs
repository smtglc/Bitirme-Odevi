using EvdeSaglik.Entity.Common;

namespace EvdeSaglik.Entity.Entities;

public class Doctor : BaseEntity
{
    public Guid UserId { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string? Bio { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsApproved { get; set; } = false;
    public DateTime? ApprovedAt { get; set; }
    public Guid? ApprovedBy { get; set; }

    //Navigasyon Propertyileri
    public User User { get; set; } = null!;
    public ICollection<DoctorWorkingHours> WorkingHours { get; set; } = new List<DoctorWorkingHours>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
