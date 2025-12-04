using EvdeSaglik.Entity.Entities;

namespace EvdeSaglik.Repositories.Interfaces;

public interface IPatientRepository : IGenericRepository<Patient>
{
    Task<Patient?> GetByUserIdAsync(Guid userId);
}
