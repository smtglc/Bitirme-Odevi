using EvdeSaglik.Entity.Enums;
using ServiceEntity = EvdeSaglik.Entity.Entities.Service;

namespace EvdeSaglik.Repositories.Interfaces;

public interface IServiceRepository : IGenericRepository<ServiceEntity>
{
    Task<IEnumerable<ServiceEntity>> GetAllActiveServicesAsync();
    Task<ServiceEntity?> GetServiceWithDoctorAsync(Guid id);
    Task<IEnumerable<ServiceEntity>> GetServicesBySpecializationAsync(MedicalSpecialization specialization);
    Task<IEnumerable<ServiceEntity>> GetFilteredServicesAsync(MedicalSpecialization? specialization, decimal? minPrice, decimal? maxPrice, Guid? doctorId);
    Task<IEnumerable<ServiceEntity>> GetServicesByDoctorIdAsync(Guid doctorId);
}
