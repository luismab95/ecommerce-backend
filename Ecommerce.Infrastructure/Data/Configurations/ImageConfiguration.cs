using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;


public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> entity)
    {
        entity.HasKey(i => i.Id);
        entity.Property(i => i.Id).ValueGeneratedOnAdd();
        entity.Property(i => i.Path).IsRequired().HasMaxLength(500);
        entity.Property(i => i.IsActive).HasDefaultValue(true);
        entity.Property(i => i.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        entity.Property(i => i.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");


        // Relationships
        entity.HasOne(i => i.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(i => i.ProductId);
        entity.HasIndex(i => new { i.ProductId, i.IsActive });
    }
}