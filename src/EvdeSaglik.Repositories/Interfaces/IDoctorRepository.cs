using EvdeSaglik.Entity.Entities;

namespace EvdeSaglik.Repositories.Interfaces;

public interface IDoctorRepository : IGenericRepository<Doctor>
{
    Task<Doctor?> GetByUserIdAsync(Guid userId);
}
