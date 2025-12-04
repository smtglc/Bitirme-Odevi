using EvdeSaglik.Entity.Entities;

namespace EvdeSaglik.Repositories.Interfaces;

public interface IDoctorDocumentRepository
{
    Task<List<DoctorDocument>> GetByDoctorIdAsync(Guid doctorId);
    Task<DoctorDocument?> GetByIdAsync(Guid id);
    Task<DoctorDocument> CreateAsync(DoctorDocument document);
    Task<DoctorDocument> UpdateAsync(DoctorDocument document);
    Task DeleteAsync(Guid id);
}
