using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain;

namespace TransactionService.Data;

public class TransactionDbContext : DbContext
{
    public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options)
    {
    }

    public DbSet<TransactionRecord> Transactions => Set<TransactionRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TransactionRecord>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.ProductCode).IsRequired().HasMaxLength(64);
            entity.Property(t => t.Quantity).IsRequired();
            entity.Property(t => t.PerformedBy).HasMaxLength(128);
            entity.Property(t => t.Notes).HasMaxLength(512);
        });
    }
}
