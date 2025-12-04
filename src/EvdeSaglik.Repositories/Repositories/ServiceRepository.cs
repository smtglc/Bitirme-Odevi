using EvdeSaglik.Entity.Enums;
using EvdeSaglik.Repositories.Data;
using EvdeSaglik.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using ServiceEntity = EvdeSaglik.Entity.Entities.Service;

namespace EvdeSaglik.Repositories.Repositories;

public class ServiceRepository : GenericRepository<ServiceEntity>, IServiceRepository
{
    public ServiceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ServiceEntity>> GetAllActiveServicesAsync()
    {
        return await _dbSet
            .Where(s => s.IsActive)
            .Include(s => s.Doctor)
            .ThenInclude(d => d.User)
            .ToListAsync();
    }

    public async Task<ServiceEntity?> GetServiceWithDoctorAsync(Guid id)
    {
        return await _dbSet
            .Include(s => s.Doctor)
            .ThenInclude(d => d.User)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<ServiceEntity>> GetServicesBySpecializationAsync(MedicalSpecialization specialization)
    {
        return await _dbSet
            .Where(s => s.IsActive && s.Specialization == specialization)
            .Include(s => s.Doctor)
            .ThenInclude(d => d.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceEntity>> GetFilteredServicesAsync(
        MedicalSpecialization? specialization, 
        decimal? minPrice, 
        decimal? maxPrice, 
        Guid? doctorId)
    {
        var query = _dbSet
            .Where(s => s.IsActive)
            .Include(s => s.Doctor)
            .ThenInclude(d => d.User)
            .AsQueryable();

        if (specialization.HasValue)
            query = query.Where(s => s.Specialization == specialization.Value);

        if (minPrice.HasValue)
            query = query.Where(s => s.BasePrice >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(s => s.BasePrice <= maxPrice.Value);

        if (doctorId.HasValue)
            query = query.Where(s => s.DoctorId == doctorId.Value);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<ServiceEntity>> GetServicesByDoctorIdAsync(Guid doctorId)
    {
        return await _dbSet
            .Where(s => s.DoctorId == doctorId)
            .Include(s => s.Doctor)
            .ThenInclude(d => d.User)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }
}
