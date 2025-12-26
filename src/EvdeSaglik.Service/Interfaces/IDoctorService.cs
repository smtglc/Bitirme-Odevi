using EvdeSaglik.Service.DTOs.Appointment;
using EvdeSaglik.Service.DTOs.Doctor;
using EvdeSaglik.Service.DTOs.Service;

namespace EvdeSaglik.Service.Interfaces;

public interface IDoctorService
{
    // Service Management
    Task<List<ServiceListDto>> GetMyServicesAsync(Guid userId);
    Task<ServiceDetailDto> CreateServiceAsync(Guid userId, CreateDoctorServiceDto dto);
    Task<ServiceDetailDto> UpdateServiceAsync(Guid userId, Guid serviceId, UpdateDoctorServiceDto dto);
    Task DeleteServiceAsync(Guid userId, Guid serviceId);
    
    // Appointment Management
    Task<List<DoctorAppointmentDto>> GetMyAppointmentsAsync(Guid userId);
    Task<List<DoctorAppointmentDto>> GetUpcomingAppointmentsAsync(Guid userId);
    Task<DoctorAppointmentDto> ConfirmAppointmentAsync(Guid userId, Guid appointmentId);
    Task<DoctorAppointmentDto> CancelAppointmentAsync(Guid userId, Guid appointmentId);
    Task<DoctorAppointmentDto> CompleteAppointmentAsync(Guid userId, Guid appointmentId);
    
    // Working Hours
    Task<List<WorkingHoursDto>> GetMyWorkingHoursAsync(Guid userId);
    Task<WorkingHoursDto> AddWorkingHoursAsync(Guid userId, CreateWorkingHoursDto dto);
    Task<WorkingHoursDto> UpdateWorkingHoursAsync(Guid userId, Guid id, CreateWorkingHoursDto dto);
    Task DeleteWorkingHoursAsync(Guid userId, Guid id);
    
    // Document Management
    Task<List<DoctorDocumentDto>> GetMyDocumentsAsync(Guid userId);
    Task<DoctorDocumentDto> GetDocumentByIdAsync(Guid userId, Guid documentId);
    Task<DoctorDocumentDto> CreateDocumentAsync(Guid userId, CreateDoctorDocumentDto dto, Microsoft.AspNetCore.Http.IFormFile? file);
    Task<DoctorDocumentDto> UpdateDocumentAsync(Guid userId, Guid documentId, UpdateDoctorDocumentDto dto, Microsoft.AspNetCore.Http.IFormFile? file);
    Task DeleteDocumentAsync(Guid userId, Guid documentId);
    
    // Profile Management
    Task<DoctorDto> GetMyProfileAsync(Guid userId);
    Task<DoctorDto> UpdateMyProfileAsync(Guid userId, UpdateDoctorProfileDto dto);
}
