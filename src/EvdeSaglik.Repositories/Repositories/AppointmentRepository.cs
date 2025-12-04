using EvdeSaglik.Entity.Entities;
using EvdeSaglik.Repositories.Data;
using EvdeSaglik.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvdeSaglik.Repositories.Repositories;

public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .Where(a => a.PatientId == patientId)
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Include(a => a.Service)
            .OrderByDescending(a => a.ScheduledDateTime)
            .ToListAsync();
    }

    public async Task<Appointment?> GetAppointmentWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Include(a => a.Service)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId)
    {
        return await _dbSet
            .Where(a => a.DoctorId == doctorId)
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Include(a => a.Service)
            .OrderByDescending(a => a.ScheduledDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsByDoctorIdAsync(Guid doctorId, DateTime fromDate)
    {
        return await _dbSet
            .Where(a => a.DoctorId == doctorId && a.ScheduledDateTime >= fromDate)
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Include(a => a.Service)
            .OrderBy(a => a.ScheduledDateTime)
            .ToListAsync();
    }
}
