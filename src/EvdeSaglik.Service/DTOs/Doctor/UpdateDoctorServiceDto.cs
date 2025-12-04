namespace EvdeSaglik.Service.DTOs.Doctor;

public class UpdateDoctorServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int DurationMinutes { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
