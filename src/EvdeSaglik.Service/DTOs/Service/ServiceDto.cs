namespace EvdeSaglik.Service.DTOs.Service;

public class ServiceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsActive { get; set; }
}
