using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain;

namespace ProductService.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => p.Code).IsUnique();
            entity.Property(p => p.Code).IsRequired().HasMaxLength(64);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(256);
            entity.Property(p => p.Category).HasMaxLength(128);
            entity.Property(p => p.UnitPrice).HasPrecision(18, 2);
            entity.Property(p => p.Stock).HasDefaultValue(0);
        });
    }
}
