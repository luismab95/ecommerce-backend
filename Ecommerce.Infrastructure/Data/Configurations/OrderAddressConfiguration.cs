using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public class OrderAddressConfiguration : IEntityTypeConfiguration<OrderAddress>
{
    public void Configure(EntityTypeBuilder<OrderAddress> entity)
    {
        entity.HasKey(oa => oa.Id);
        entity.Property(oa => oa.Id).ValueGeneratedOnAdd();
        entity.Property(oa => oa.ShippingStreet).IsRequired().HasMaxLength(200);
        entity.Property(oa => oa.ShippingCity).IsRequired().HasMaxLength(100);
        entity.Property(oa => oa.ShippingState).IsRequired().HasMaxLength(100);
        entity.Property(oa => oa.ShippingZipCode).IsRequired().HasMaxLength(20);
        entity.Property(oa => oa.ShippingCountry).IsRequired().HasMaxLength(100);
        entity.Property(oa => oa.ShippingPhone).IsRequired().HasMaxLength(20);
        entity.Property(oa => oa.ShippingEmail).IsRequired().HasMaxLength(255);
        entity.Property(oa => oa.UseSameAddressForBilling).IsRequired().HasDefaultValue(true);
        entity.Property(oa => oa.BillingStreet).IsRequired().HasMaxLength(200);
        entity.Property(oa => oa.BillingCity).IsRequired().HasMaxLength(100);
        entity.Property(oa => oa.BillingState).IsRequired().HasMaxLength(100);
        entity.Property(oa => oa.BillingZipCode).IsRequired().HasMaxLength(20);
        entity.Property(oa => oa.BillingCountry).IsRequired().HasMaxLength(100);
        entity.Property(oa => oa.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        entity.Property(oa => oa.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        entity.HasOne(oa => oa.Order)
            .WithOne(o => o.OrderAddress)
            .HasForeignKey<OrderAddress>(oa => oa.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(oa => oa.OrderId).IsUnique();
        entity.HasIndex(oa => oa.ShippingCountry);
        entity.HasIndex(oa => oa.ShippingState);
        entity.HasIndex(oa => oa.ShippingCity);
        entity.HasIndex(oa => oa.ShippingEmail);
        entity.HasIndex(oa => oa.ShippingPhone);
        entity.HasIndex(oa => oa.CreatedAt);
    }
}