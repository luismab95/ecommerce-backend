using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> entity)
    {
        entity.HasKey(ua => ua.Id);
        entity.Property(ua => ua.Id).ValueGeneratedOnAdd();
        entity.Property(ua => ua.UseSameAddressForBilling).HasDefaultValue(true);
        entity.Property(ua => ua.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        entity.Property(ua => ua.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        entity.HasOne(ua => ua.User)
            .WithOne(u => u.UserAddress)
            .HasForeignKey<UserAddress>(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(ua => ua.UserId).IsUnique();
    }
}