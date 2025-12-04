using EvdeSaglik.Entity.Entities;
using EvdeSaglik.Repositories.Data;
using EvdeSaglik.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvdeSaglik.Repositories.Repositories;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Patient?> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }
}
