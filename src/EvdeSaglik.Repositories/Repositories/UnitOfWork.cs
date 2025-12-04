using EvdeSaglik.Repositories.Data;
using EvdeSaglik.Repositories.Interfaces;

namespace EvdeSaglik.Repositories.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IPatientRepository? _patientRepository;
    private IDoctorRepository? _doctorRepository;
    private IServiceRepository? _serviceRepository;
    private IAppointmentRepository? _appointmentRepository;
    private IMessageRepository? _messageRepository;
    private IWorkingHoursRepository? _workingHoursRepository;
    private IDoctorDocumentRepository? _doctorDocumentRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IPatientRepository Patients => _patientRepository ??= new PatientRepository(_context);
    
    public IDoctorRepository Doctors => _doctorRepository ??= new DoctorRepository(_context);
    
    public IServiceRepository Services => _serviceRepository ??= new ServiceRepository(_context);
    
    public IAppointmentRepository Appointments => _appointmentRepository ??= new AppointmentRepository(_context);
    
    public IMessageRepository Messages => _messageRepository ??= new MessageRepository(_context);

    public IWorkingHoursRepository WorkingHours => _workingHoursRepository ??= new WorkingHoursRepository(_context);

    public IDoctorDocumentRepository DoctorDocuments => _doctorDocumentRepository ??= new DoctorDocumentRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
