using Microsoft.AspNetCore.Identity;

namespace EvdeSaglik.Entity.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}
