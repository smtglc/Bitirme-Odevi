namespace EvdeSaglik.Service.DTOs.Appointment;

public class CreateAppointmentDto
{
    public Guid ServiceId { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public string? PatientNotes { get; set; }
}
