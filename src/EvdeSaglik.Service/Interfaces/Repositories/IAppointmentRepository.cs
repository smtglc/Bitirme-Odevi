using EvdeSaglik.Entity.Entities;

namespace EvdeSaglik.Service.Interfaces.Repositories;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId);
    Task<Appointment?> GetAppointmentWithDetailsAsync(Guid id);
}
