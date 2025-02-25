using Microsoft.EntityFrameworkCore;

public class AutoDoDbContext : DbContext
{
    // Propriété DbSet pour les profils
    public DbSet<Profil> Profils { get; set; }

    private readonly string _connectionString;

    public AutoDoDbContext(IConnectionStringProvider connectionStringProvider)
    {
        _connectionString = connectionStringProvider.Get("DefaultConnection");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectionString);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profil>(entity =>
        {
            entity.HasKey(p => p.Uuid);
            entity.Property(p => p.Ratehour).IsRequired();
            entity.Property(p => p.CV).IsRequired();
            entity.Property(p => p.CV_Date).IsRequired();
            entity.Property(p => p.Job_title).IsRequired();
            entity.Property(p => p.Experience_level).IsRequired();
            entity.Property(p => p.Skills)
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            entity.Property(p => p.Keywords)
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );
        });
    }

}

