using EvdeSaglik.Entity.Entities;
using ServiceEntity = EvdeSaglik.Entity.Entities.Service;

namespace EvdeSaglik.Service.Interfaces.Repositories;

public interface IServiceRepository : IGenericRepository<ServiceEntity>
{
    Task<IEnumerable<ServiceEntity>> GetAllActiveServicesAsync();
    Task<ServiceEntity?> GetServiceWithDoctorAsync(Guid id);
}
