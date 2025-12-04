using EvdeSaglik.Entity.Entities;

namespace EvdeSaglik.Service.Interfaces.Repositories;

public interface IDoctorRepository : IGenericRepository<Doctor>
{
    Task<Doctor?> GetByUserIdAsync(Guid userId);
}
