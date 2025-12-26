using EvdeSaglik.Entity.Entities;

namespace EvdeSaglik.Repositories.Interfaces;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId);
    Task<Appointment?> GetAppointmentWithDetailsAsync(Guid id);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId);
    Task<IEnumerable<Appointment>> GetUpcomingAppointmentsByDoctorIdAsync(Guid doctorId, DateTime fromDate);
    Task<bool> HasAppointmentsForServiceAsync(Guid serviceId);
}
