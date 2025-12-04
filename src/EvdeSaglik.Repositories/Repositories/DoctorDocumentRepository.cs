using EvdeSaglik.Entity.Entities;
using EvdeSaglik.Repositories.Data;
using EvdeSaglik.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvdeSaglik.Repositories.Repositories;

public class DoctorDocumentRepository : IDoctorDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public DoctorDocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DoctorDocument>> GetByDoctorIdAsync(Guid doctorId)
    {
        return await _context.DoctorDocuments
            .Where(d => d.DoctorId == doctorId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<DoctorDocument?> GetByIdAsync(Guid id)
    {
        return await _context.DoctorDocuments
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<DoctorDocument> CreateAsync(DoctorDocument document)
    {
        await _context.DoctorDocuments.AddAsync(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task<DoctorDocument> UpdateAsync(DoctorDocument document)
    {
        _context.DoctorDocuments.Update(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task DeleteAsync(Guid id)
    {
        var document = await GetByIdAsync(id);
        if (document != null)
        {
            _context.DoctorDocuments.Remove(document);
            await _context.SaveChangesAsync();
        }
    }
}
