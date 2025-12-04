namespace EvdeSaglik.Service.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IPatientRepository Patients { get; }
    IDoctorRepository Doctors { get; }
    IServiceRepository Services { get; }
    IAppointmentRepository Appointments { get; }
    IMessageRepository Messages { get; }
    
    Task<int> SaveChangesAsync();
}
