using EvdeSaglik.Entity.Entities;
using EvdeSaglik.Repositories.Data;
using EvdeSaglik.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvdeSaglik.Repositories.Repositories;

public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
{
    public DoctorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Doctor?> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.UserId == userId);
    }
}
