using EvdeSaglik.Service.DTOs.Service;

namespace EvdeSaglik.Service.Interfaces;

public interface IServiceManagementService
{
    Task<List<ServiceListDto>> GetAllServicesAsync();
    Task<ServiceDetailDto> GetServiceDetailAsync(Guid id);
    Task<List<ServiceListDto>> GetFilteredServicesAsync(ServiceFilterDto filter);
}
