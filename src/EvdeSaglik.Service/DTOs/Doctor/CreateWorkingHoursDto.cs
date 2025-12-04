namespace EvdeSaglik.Service.DTOs.Doctor;

public class CreateWorkingHoursDto
{
    public int DayOfWeek { get; set; } 
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
}
