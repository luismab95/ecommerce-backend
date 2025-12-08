using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public class WishListConfiguration : IEntityTypeConfiguration<WishList>
{
    public void Configure(EntityTypeBuilder<WishList> entity)
    {
        entity.HasKey(w => w.Id);
        entity.Property(w => w.Id).ValueGeneratedOnAdd();
        entity.Property(w => w.IsActive).HasDefaultValue(true);
        entity.Property(w => w.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        entity.Property(w => w.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");


        // Relationships
        entity.HasOne(w => w.Product)
            .WithMany(p => p.WishLists)
            .HasForeignKey(w => w.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(w => w.User)
            .WithMany(u => u.WishLists)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(w => w.ProductId);
        entity.HasIndex(w => w.UserId);
        entity.HasIndex(w => new { w.ProductId, w.UserId }).IsUnique();
        entity.HasIndex(w => w.IsActive);
    }
}