using EvdeSaglik.Entity.Common;

namespace EvdeSaglik.Entity.Entities;

public class DoctorDocument : BaseEntity
{
    public Guid DoctorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? FilePath { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }

    // Navigation Property
    public Doctor Doctor { get; set; } = null!;
}
