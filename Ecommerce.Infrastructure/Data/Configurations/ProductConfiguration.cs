using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> entity)
    {
        entity.HasKey(p => p.Id);
        entity.Property(p => p.Id).ValueGeneratedOnAdd();
        entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
        entity.Property(p => p.Description).IsRequired().HasMaxLength(1000);
        entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(p => p.Stock).IsRequired().HasDefaultValue(0);
        entity.Property(p => p.Featured).HasDefaultValue(false);
        entity.Property(p => p.IsActive).HasDefaultValue(true);
        entity.Property(p => p.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        entity.Property(p => p.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        entity.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        entity.HasIndex(p => p.Name).IsUnique();
        entity.HasIndex(p => p.CategoryId);
        entity.HasIndex(p => p.Featured);
        entity.HasIndex(p => p.IsActive);
        entity.HasIndex(p => new { p.CategoryId, p.IsActive });
        entity.HasIndex(p => new { p.Name, p.Description });
        entity.HasIndex(p => new { p.Featured, p.IsActive });
        entity.HasIndex(p => p.Price);
    }
}