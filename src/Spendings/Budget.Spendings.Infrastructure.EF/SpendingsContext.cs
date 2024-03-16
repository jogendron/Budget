using Microsoft.EntityFrameworkCore;

namespace Budget.Spendings.Infrastructure.EF;

public class SpendingsContext: DbContext
{
    public SpendingsContext(DbContextOptions<SpendingsContext> options) : base(options)
    {
        SpendingCategories = Set<SpendingCategory>();
        Spendings = Set<Spending>();
    }

    public DbSet<SpendingCategory> SpendingCategories { get; }

    public DbSet<Spending> Spendings { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SpendingCategory>(builder => {
            builder.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Spending>(builder => {
            builder.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Event>(builder =>
        {
            builder.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.HasDefaultSchema("Spendings");
    }
}