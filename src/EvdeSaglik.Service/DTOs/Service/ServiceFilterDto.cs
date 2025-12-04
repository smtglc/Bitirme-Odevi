namespace EvdeSaglik.Service.DTOs.Service;

public class ServiceFilterDto
{
    public string? Specialization { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public Guid? DoctorId { get; set; }
}
