namespace EvdeSaglik.Service.DTOs.Patient;

public class UpdatePatientDto
{
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? MedicalHistory { get; set; }
    public string? EmergencyContact { get; set; }
}
