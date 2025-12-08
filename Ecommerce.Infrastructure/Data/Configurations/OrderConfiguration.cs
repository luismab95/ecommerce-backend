using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> entity)
    {
        entity.HasKey(o => o.Id);
        entity.Property(o => o.Id).ValueGeneratedOnAdd();
        entity.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
        entity.Property(o => o.Subtotal).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(o => o.Total).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(o => o.Tax).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(o => o.ShippingCost).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(o => o.Discount).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);
        entity.Property(o => o.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        entity.Property(o => o.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");


        // Relationships
        entity.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(o => o.OrderPayment)
            .WithOne(op => op.Order)
            .HasForeignKey<OrderPayment>(op => op.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(o => o.OrderStatus)
            .WithOne(os => os.Order)
            .HasForeignKey<OrderStatus>(os => os.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(o => o.OrderAddress)
            .WithOne(oa => oa.Order)
            .HasForeignKey<OrderAddress>(oa => oa.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(o => o.OrderNumber).IsUnique();
        entity.HasIndex(o => new { o.UserId, o.CreatedAt });
        entity.HasIndex(o => o.UserId);
        entity.HasIndex(o => o.CreatedAt);
        entity.HasIndex(o => o.Total);
    }
}