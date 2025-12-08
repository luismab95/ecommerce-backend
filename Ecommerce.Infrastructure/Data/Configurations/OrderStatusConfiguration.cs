using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> entity)
    {
        entity.HasKey(os => os.Id);
        entity.Property(os => os.Id).ValueGeneratedOnAdd();
        entity.Property(os => os.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        entity.Property(os => os.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        entity.Property(os => os.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        entity.HasOne(os => os.Order)
            .WithOne(o => o.OrderStatus)
            .HasForeignKey<OrderStatus>(os => os.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(os => os.OrderId).IsUnique();
        entity.HasIndex(os => os.Status);
        entity.HasIndex(os => new { os.OrderId, os.Status });
        entity.HasIndex(os => os.CreatedAt);
    }
}