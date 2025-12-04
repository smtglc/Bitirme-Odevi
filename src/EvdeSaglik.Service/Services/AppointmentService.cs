using EvdeSaglik.Entity.Enums;
using EvdeSaglik.Service.DTOs.Appointment;
using EvdeSaglik.Service.Interfaces;
using EvdeSaglik.Repositories.Interfaces;
using EvdeSaglik.Entity.Entities;

namespace EvdeSaglik.Service.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public AppointmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<AppointmentDto>> GetMyAppointmentsAsync(Guid userId)
    {
        var patient = await _unitOfWork.Patients.GetByUserIdAsync(userId);
        if (patient == null)
            throw new Exception("Patient record not found. Please run fix_existing_users.sql script or re-register.");

        var appointments = await _unitOfWork.Appointments.GetByPatientIdAsync(patient.Id);
        
        return appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            PatientId = a.PatientId,
            PatientName = a.Patient.User.FirstName + " " + a.Patient.User.LastName,
            DoctorId = a.DoctorId,
            DoctorName = a.Doctor.User.FirstName + " " + a.Doctor.User.LastName,
            ServiceId = a.ServiceId,
            ServiceName = a.Service.Name,
            ScheduledDateTime = a.ScheduledDateTime,
            DurationMinutes = a.DurationMinutes,
            Status = a.Status.ToString(),
            TotalAmount = a.TotalAmount,
            PatientNotes = a.PatientNotes
        }).ToList();
    }

    public async Task<AppointmentDto> CreateAppointmentAsync(Guid userId, CreateAppointmentDto dto)
    {
        var patient = await _unitOfWork.Patients.GetByUserIdAsync(userId);
        if (patient == null)
            throw new Exception("Patient record not found. Please run fix_existing_users.sql script or re-register.");

        var service = await _unitOfWork.Services.GetServiceWithDoctorAsync(dto.ServiceId);
        if (service == null)
            throw new Exception("Service not found");

        var appointment = new Appointment
        {
            PatientId = patient.Id,
            DoctorId = service.DoctorId,
            ServiceId = service.Id,
            ScheduledDateTime = dto.ScheduledDateTime,
            DurationMinutes = service.DurationMinutes,
            Status = AppointmentStatus.Pending,
            PatientNotes = dto.PatientNotes,
            TotalAmount = service.BasePrice
        };

        await _unitOfWork.Appointments.AddAsync(appointment);
        await _unitOfWork.SaveChangesAsync();

        return new AppointmentDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            PatientName = patient.User.FirstName + " " + patient.User.LastName,
            DoctorId = appointment.DoctorId,
            DoctorName = service.Doctor.User.FirstName + " " + service.Doctor.User.LastName,
            ServiceId = appointment.ServiceId,
            ServiceName = service.Name,
            ScheduledDateTime = appointment.ScheduledDateTime,
            DurationMinutes = appointment.DurationMinutes,
            Status = appointment.Status.ToString(),
            TotalAmount = appointment.TotalAmount,
            PatientNotes = appointment.PatientNotes
        };
    }

    public async Task<AppointmentDto> GetAppointmentDetailAsync(Guid id)
    {
        var appointment = await _unitOfWork.Appointments.GetAppointmentWithDetailsAsync(id);
        if (appointment == null)
            throw new Exception("Appointment not found");

        return new AppointmentDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            PatientName = appointment.Patient.User.FirstName + " " + appointment.Patient.User.LastName,
            DoctorId = appointment.DoctorId,
            DoctorName = appointment.Doctor.User.FirstName + " " + appointment.Doctor.User.LastName,
            ServiceId = appointment.ServiceId,
            ServiceName = appointment.Service.Name,
            ScheduledDateTime = appointment.ScheduledDateTime,
            DurationMinutes = appointment.DurationMinutes,
            Status = appointment.Status.ToString(),
            TotalAmount = appointment.TotalAmount,
            PatientNotes = appointment.PatientNotes
        };
    }

    public async Task<AppointmentDto> CancelAppointmentAsync(Guid appointmentId, Guid userId)
    {
        var appointment = await _unitOfWork.Appointments.GetAppointmentWithDetailsAsync(appointmentId);
        if (appointment == null)
            throw new Exception("Appointment not found");

        var patient = await _unitOfWork.Patients.GetByUserIdAsync(userId);
        if (patient == null)
            throw new Exception("Patient record not found. Please run fix_existing_users.sql script or re-register.");

        if (appointment.PatientId != patient.Id)
            throw new UnauthorizedAccessException("You can only cancel your own appointments");

      
        if (appointment.Status == AppointmentStatus.Cancelled)
            throw new Exception("Appointment is already cancelled");

        if (appointment.Status == AppointmentStatus.Completed)
            throw new Exception("Cannot cancel a completed appointment");

        appointment.Status = AppointmentStatus.Cancelled;
        await _unitOfWork.SaveChangesAsync();

        return new AppointmentDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            PatientName = appointment.Patient.User.FirstName + " " + appointment.Patient.User.LastName,
            DoctorId = appointment.DoctorId,
            DoctorName = appointment.Doctor.User.FirstName + " " + appointment.Doctor.User.LastName,
            ServiceId = appointment.ServiceId,
            ServiceName = appointment.Service.Name,
            ScheduledDateTime = appointment.ScheduledDateTime,
            DurationMinutes = appointment.DurationMinutes,
            Status = appointment.Status.ToString(),
            TotalAmount = appointment.TotalAmount,
            PatientNotes = appointment.PatientNotes
        };
    }
}
