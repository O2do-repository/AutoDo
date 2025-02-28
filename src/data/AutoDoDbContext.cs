﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
public class AutoDoDbContext : DbContext
{
    public DbSet<Profil> Profils { get; set; }

    private readonly string? _connectionString;
    private readonly bool _useInMemory;

    // Constructeur pour l'application
    public AutoDoDbContext(IConnectionStringProvider connectionStringProvider)
    {
        _connectionString = connectionStringProvider.Get("DefaultConnection");
        _useInMemory = false;
    }

    // Constructeur pour les tests (utilise InMemory)
    public AutoDoDbContext(DbContextOptions<AutoDoDbContext> options) : base(options)
    {
        _useInMemory = true;
    }

    // Ajout d'un constructeur sans paramètre pour éviter l'erreur avec EF CLI
    public AutoDoDbContext() : base(new DbContextOptions<AutoDoDbContext>())
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            if (!_useInMemory && !string.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseSqlite(_connectionString);
            }
        }
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
            
            entity.Property(p => p.Experience_level)
                .HasConversion(
                    v => v.ToString(),  // Convertir l'enum en string
                    v => Enum.Parse<Experience>(v)  // Convertir le string en enum
                );



            // Conversion de la liste de Skills en string (CSV)
            entity.Property(p => p.Skills)
                .HasConversion(
                    v => string.Join(",", v),  // Liste -> string
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() // String -> Liste
                );

            // Conversion de la liste de Keywords en string (CSV)
            entity.Property(p => p.Keywords)
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );
        });
    }
}
