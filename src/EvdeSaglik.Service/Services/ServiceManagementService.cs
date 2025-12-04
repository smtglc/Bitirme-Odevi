using EvdeSaglik.Entity.Enums;
using EvdeSaglik.Service.DTOs.Service;
using EvdeSaglik.Service.Interfaces;
using EvdeSaglik.Repositories.Interfaces;

namespace EvdeSaglik.Service.Services;

public class ServiceManagementService : IServiceManagementService
{
    private readonly IUnitOfWork _unitOfWork;

    public ServiceManagementService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ServiceListDto>> GetAllServicesAsync()
    {
        var services = await _unitOfWork.Services.GetAllActiveServicesAsync();
        
        return services.Select(s => new ServiceListDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            BasePrice = s.BasePrice,
            DurationMinutes = s.DurationMinutes,
            Specialization = s.Specialization.ToString(),
            DoctorName = s.Doctor.User.FirstName + " " + s.Doctor.User.LastName,
            DoctorSpecialization = s.Doctor.Specialization
        }).ToList();
    }

    public async Task<ServiceDetailDto> GetServiceDetailAsync(Guid id)
    {
        var service = await _unitOfWork.Services.GetServiceWithDoctorAsync(id);
        
        if (service == null)
            throw new Exception("Service not found");

        return new ServiceDetailDto
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            BasePrice = service.BasePrice,
            DurationMinutes = service.DurationMinutes,
            Specialization = service.Specialization.ToString(),
            DoctorId = service.DoctorId,
            DoctorUserId = service.Doctor.UserId,
            DoctorName = service.Doctor.User.FirstName + " " + service.Doctor.User.LastName,
            DoctorEmail = service.Doctor.User.Email!,
            DoctorPhone = service.Doctor.User.PhoneNumber!,
            DoctorSpecialization = service.Doctor.Specialization,
            DoctorBio = service.Doctor.Bio,
            DoctorExperience = service.Doctor.YearsOfExperience
        };
    }

    public async Task<List<ServiceListDto>> GetFilteredServicesAsync(ServiceFilterDto filter)
    {
        MedicalSpecialization? specialization = null;
        if (!string.IsNullOrEmpty(filter.Specialization))
        {
            if (Enum.TryParse<MedicalSpecialization>(filter.Specialization, true, out var parsedSpec))
            {
                specialization = parsedSpec;
            }
        }

        var services = await _unitOfWork.Services.GetFilteredServicesAsync(
            specialization,
            filter.MinPrice,
            filter.MaxPrice,
            filter.DoctorId
        );

        return services.Select(s => new ServiceListDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            BasePrice = s.BasePrice,
            DurationMinutes = s.DurationMinutes,
            Specialization = s.Specialization.ToString(),
            DoctorName = s.Doctor.User.FirstName + " " + s.Doctor.User.LastName,
            DoctorSpecialization = s.Doctor.Specialization
        }).ToList();
    }
}
