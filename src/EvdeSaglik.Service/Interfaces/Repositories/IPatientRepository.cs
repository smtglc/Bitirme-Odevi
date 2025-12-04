using EvdeSaglik.Entity.Entities;

namespace EvdeSaglik.Service.Interfaces.Repositories;

public interface IPatientRepository : IGenericRepository<Patient>
{
    Task<Patient?> GetByUserIdAsync(Guid userId);
}
