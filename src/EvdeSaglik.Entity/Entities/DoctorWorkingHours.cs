using EvdeSaglik.Entity.Common;

namespace EvdeSaglik.Entity.Entities;

public class DoctorWorkingHours : BaseEntity
{
    public Guid DoctorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;

 
    public Doctor Doctor { get; set; } = null!;
}
