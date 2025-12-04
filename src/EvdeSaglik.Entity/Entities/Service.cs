using EvdeSaglik.Entity.Common;
using EvdeSaglik.Entity.Enums;

namespace EvdeSaglik.Entity.Entities;

public class Service : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsActive { get; set; } = true;
    public MedicalSpecialization Specialization { get; set; }
    public Guid DoctorId { get; set; }

   
    public Doctor Doctor { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
