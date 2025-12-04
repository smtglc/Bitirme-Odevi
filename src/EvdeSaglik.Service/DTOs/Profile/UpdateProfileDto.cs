namespace EvdeSaglik.Service.DTOs.Profile;

public class UpdateProfileDto
{
    public string? PhoneNumber { get; set; }
    
   
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? EmergencyContact { get; set; }
    
   
    public string? Bio { get; set; }
}
