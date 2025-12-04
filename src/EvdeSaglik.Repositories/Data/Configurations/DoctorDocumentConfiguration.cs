using EvdeSaglik.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvdeSaglik.Repositories.Data.Configurations;

public class DoctorDocumentConfiguration : IEntityTypeConfiguration<DoctorDocument>
{
    public void Configure(EntityTypeBuilder<DoctorDocument> builder)
    {
        builder.ToTable("DoctorDocuments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Description)
            .HasMaxLength(2000);

        builder.Property(d => d.FilePath)
            .HasMaxLength(500);

        builder.Property(d => d.FileType)
            .HasMaxLength(100);

        builder.Property(d => d.DoctorId)
            .IsRequired();

        // Relationships
        builder.HasOne(d => d.Doctor)
            .WithMany()
            .HasForeignKey(d => d.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
