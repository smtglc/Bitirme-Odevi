namespace EvdeSaglik.Service.DTOs.Profile;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    
   
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? EmergencyContact { get; set; }
    
 
    public string? Specialization { get; set; }
    public string? LicenseNumber { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? Bio { get; set; }
    public bool? IsApproved { get; set; }
}
