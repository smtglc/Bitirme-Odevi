using EvdeSaglik.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvdeSaglik.Repositories.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");
        builder.Property(p => p.PaymentMethod).HasMaxLength(100);
        builder.Property(p => p.TransactionId).HasMaxLength(200);
        
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
    }
}
