namespace EvdeSaglik.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPatientRepository Patients { get; }
    IDoctorRepository Doctors { get; }
    IServiceRepository Services { get; }
    IAppointmentRepository Appointments { get; }
    IMessageRepository Messages { get; }
    IWorkingHoursRepository WorkingHours { get; }
    IDoctorDocumentRepository DoctorDocuments { get; }
    
    Task<int> SaveChangesAsync();
}
