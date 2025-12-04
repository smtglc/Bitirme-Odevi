using EvdeSaglik.Entity.Entities;
using EvdeSaglik.Repositories.Data;
using EvdeSaglik.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvdeSaglik.Repositories.Repositories;

public class WorkingHoursRepository : GenericRepository<DoctorWorkingHours>, IWorkingHoursRepository
{
    public WorkingHoursRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<DoctorWorkingHours>> GetByDoctorIdAsync(Guid doctorId)
    {
        return await _dbSet
            .Where(wh => wh.DoctorId == doctorId)
            .OrderBy(wh => wh.DayOfWeek)
            .ThenBy(wh => wh.StartTime)
            .ToListAsync();
    }

    public async Task<DoctorWorkingHours?> GetByDoctorAndDayAsync(Guid doctorId, DayOfWeek day)
    {
        return await _dbSet
            .FirstOrDefaultAsync(wh => wh.DoctorId == doctorId && wh.DayOfWeek == day);
    }
}
