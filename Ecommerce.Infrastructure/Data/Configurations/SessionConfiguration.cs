using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;


public class SessionConfiguration : IEntityTypeConfiguration<Session>
{

    public void Configure(EntityTypeBuilder<Session> entity)
    {

        entity.HasKey(s => s.Id);
        entity.Property(s => s.Id).ValueGeneratedOnAdd();
        entity.Property(s => s.DeviceInfo).HasMaxLength(500);
        entity.Property(s => s.IpAddress).HasMaxLength(45);
        entity.Property(s => s.RefreshToken).IsRequired();
        entity.Property(s => s.Revoked).HasDefaultValue(false);
        entity.Property(s => s.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        entity.Property(s => s.ExpiresAt).IsRequired().HasDefaultValueSql("GETUTCDATE() + 14");

        // Indexes
        entity.HasIndex(s => s.UserId);
        entity.HasIndex(s => new { s.UserId, s.Revoked });

        // Relationships
        entity.HasOne(s => s.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}