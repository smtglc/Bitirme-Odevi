namespace EvdeSaglik.Service.DTOs.Doctor;

public class UpdateDoctorProfileDto
{
    public string Specialization { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string? Bio { get; set; }
}
