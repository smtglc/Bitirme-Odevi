namespace EvdeSaglik.Service.DTOs.Service;

public class ServiceDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int DurationMinutes { get; set; }
    public string Specialization { get; set; } = string.Empty;
    
   
    public Guid DoctorId { get; set; }
    public Guid DoctorUserId { get; set; }  
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorEmail { get; set; } = string.Empty;
    public string DoctorPhone { get; set; } = string.Empty;
    public string DoctorSpecialization { get; set; } = string.Empty;
    public string? DoctorBio { get; set; }
    public int DoctorExperience { get; set; }
}
