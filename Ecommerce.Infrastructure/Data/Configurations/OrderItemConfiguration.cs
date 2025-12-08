using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> entity)
    {
        entity.HasKey(oi => oi.Id);
        entity.Property(oi => oi.Id).ValueGeneratedOnAdd();
        entity.Property(oi => oi.ProductName).IsRequired().HasMaxLength(200);
        entity.Property(oi => oi.Subtotal).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(oi => oi.Price).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(oi => oi.Quantity).IsRequired().HasDefaultValue(1);
        entity.Property(oi => oi.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        entity.Property(oi => oi.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        entity.HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(oi => oi.OrderId);
        entity.HasIndex(oi => oi.ProductId);
        entity.HasIndex(oi => new { oi.OrderId, oi.ProductId });
        entity.HasIndex(oi => oi.Price);
    }
}