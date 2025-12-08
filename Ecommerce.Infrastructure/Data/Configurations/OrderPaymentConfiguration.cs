using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public class OrderPaymentConfiguration : IEntityTypeConfiguration<OrderPayment>
{
    public void Configure(EntityTypeBuilder<OrderPayment> entity)
    {
        entity.HasKey(op => op.Id);
        entity.Property(op => op.Id).ValueGeneratedOnAdd();
        entity.Property(op => op.PaymentGatewayId).IsRequired().HasMaxLength(100);
        entity.Property(op => op.CardHolderName).IsRequired().HasMaxLength(200);
        entity.Property(op => op.CardLastFour).IsRequired().HasMaxLength(4);
        entity.Property(op => op.Method).IsRequired().HasMaxLength(50);
        entity.Property(op => op.Amount).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(op => op.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        entity.Property(op => op.PaidAt).IsRequired(false);
        entity.Property(op => op.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        entity.Property(op => op.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        entity.HasOne(op => op.Order)
            .WithOne(o => o.OrderPayment)
            .HasForeignKey<OrderPayment>(op => op.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(op => op.OrderId).IsUnique();
        entity.HasIndex(op => op.PaymentGatewayId).IsUnique();
        entity.HasIndex(op => op.Method);
        entity.HasIndex(op => op.Status);
        entity.HasIndex(op => op.CreatedAt);
    }
}