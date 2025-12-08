using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;


public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{

    public void Configure(EntityTypeBuilder<Category> entity)
    {

        entity.HasKey(c => c.Id);
        entity.Property(c => c.Id).ValueGeneratedOnAdd();
        entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
        entity.Property(c => c.Description).IsRequired().HasMaxLength(255);
        entity.Property(c => c.IsActive).HasDefaultValue(true);
        entity.Property(c => c.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        entity.Property(c => c.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        entity.HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        //Indexes
        entity.HasIndex(c => c.Name).IsUnique();
        entity.HasIndex(c => new { c.Name, c.Description, c.IsActive });
    }
}