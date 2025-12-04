using EvdeSaglik.Entity.Entities;

namespace EvdeSaglik.Repositories.Interfaces;

public interface IWorkingHoursRepository : IGenericRepository<DoctorWorkingHours>
{
    Task<IEnumerable<DoctorWorkingHours>> GetByDoctorIdAsync(Guid doctorId);
    Task<DoctorWorkingHours?> GetByDoctorAndDayAsync(Guid doctorId, DayOfWeek day);
}
