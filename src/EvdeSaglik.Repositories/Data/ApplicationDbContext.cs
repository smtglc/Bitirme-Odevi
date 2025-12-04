using EvdeSaglik.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServiceEntity = EvdeSaglik.Entity.Entities.Service;

namespace EvdeSaglik.Repositories.Data;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<DoctorWorkingHours> DoctorWorkingHours { get; set; }
    public DbSet<DoctorDocument> DoctorDocuments { get; set; }
    public DbSet<ServiceEntity> Services { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply configurations
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Seed Roles
        var patientRoleId = Guid.NewGuid();
        var doctorRoleId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();

        builder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid>
            {
                Id = patientRoleId,
                Name = "Patient",
                NormalizedName = "PATIENT",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new IdentityRole<Guid>
            {
                Id = doctorRoleId,
                Name = "Doctor",
                NormalizedName = "DOCTOR",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new IdentityRole<Guid>
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );

        // Seed Admin User
        var adminUserId = Guid.NewGuid();
        var hasher = new PasswordHasher<User>();

        var adminUser = new User
        {
            Id = adminUserId,
            UserName = "admin@evdesaglik.com",
            NormalizedUserName = "ADMIN@EVDESAGLIK.COM",
            Email = "admin@evdesaglik.com",
            NormalizedEmail = "ADMIN@EVDESAGLIK.COM",
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "User",
            PhoneNumber = "5551234567",
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

        builder.Entity<User>().HasData(adminUser);

        // Assign Admin Role to Admin User
        builder.Entity<IdentityUserRole<Guid>>().HasData(
            new IdentityUserRole<Guid>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            }
        );
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Entity.Common.BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (Entity.Common.BaseEntity)entry.Entity;
            
            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}
