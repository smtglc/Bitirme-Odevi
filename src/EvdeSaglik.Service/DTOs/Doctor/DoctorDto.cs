namespace EvdeSaglik.Service.DTOs.Doctor;

public class DoctorDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string? Bio { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
