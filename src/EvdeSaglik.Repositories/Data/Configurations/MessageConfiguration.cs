using EvdeSaglik.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvdeSaglik.Repositories.Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Content).IsRequired().HasMaxLength(2000);

        builder.HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Service)
            .WithMany()
            .HasForeignKey(m => m.ServiceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(m => new { m.SenderId, m.ReceiverId });
        builder.HasIndex(m => m.CreatedAt);
    }
}
