using EvdeSaglik.Service.DTOs.Appointment;

namespace EvdeSaglik.Service.Interfaces;

public interface IAppointmentService
{
    Task<List<AppointmentDto>> GetMyAppointmentsAsync(Guid userId);
    Task<AppointmentDto> CreateAppointmentAsync(Guid userId, CreateAppointmentDto dto);
    Task<AppointmentDto> GetAppointmentDetailAsync(Guid id);
    Task<AppointmentDto> CancelAppointmentAsync(Guid appointmentId, Guid userId);
}
