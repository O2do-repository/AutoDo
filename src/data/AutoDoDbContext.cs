using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class AutoDoDbContext : DbContext
{
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Consultant> Consultants { get; set; }
    public DbSet<Matching> Matchings { get; set; } 
    public DbSet<RFP> Rfps { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Keyword> Keywords { get; set; }
    

    private readonly bool _useInMemory;
    private readonly IConnectionStringProvider _connectionStringProvider;

    public AutoDoDbContext(IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
        _useInMemory = false;
    }

    public AutoDoDbContext(DbContextOptions<AutoDoDbContext> options) : base(options)
    {
        _useInMemory = true;
    }

    public AutoDoDbContext() : base(new DbContextOptions<AutoDoDbContext>())
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer("Server=O2DO_LAPTOP_2\\SQLEXPRESS;Database=AutoDo;Trusted_Connection=True;TrustServerCertificate=True;");
    }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuration de RFP
        modelBuilder.Entity<RFP>(entity =>
        {
            entity.ToTable("RFP");
            entity.HasKey(r => r.RFPUuid);
            entity.Property(r => r.RFPUuid).HasColumnName("RFPUuid").IsRequired();
            entity.Property(r => r.DeadlineDate).HasColumnName("DeadlineDate").IsRequired();
            entity.Property(r => r.DescriptionBrut).HasColumnName("DescriptionBrut").HasMaxLength(5000);
            entity.Property(r => r.ExperienceLevel).HasConversion<string>();
            entity.Property(r => r.RfpPriority).HasColumnName("RfpPriority");
            entity.Property(r => r.PublicationDate).HasColumnName("PublicationDate").IsRequired();
            entity.Property(r => r.JobTitle).HasColumnName("JobTitle").HasMaxLength(255);
            entity.Property(r => r.RfpUrl).HasColumnName("RfpUrl").HasMaxLength(500);
            entity.Property(r => r.Workplace).HasColumnName("Workplace").HasMaxLength(255);
            entity.Property(r => r.Reference).HasColumnName("Reference").HasMaxLength(255);
        });


        // Configuration de Consultant
        modelBuilder.Entity<Consultant>(entity =>
        {
            entity.ToTable("Consultant");
            entity.HasKey(c => c.ConsultantUuid);
            entity.Property(c => c.ConsultantUuid).HasColumnName("ConsultantUuid").IsRequired();
            entity.Property(c => c.Email).HasColumnName("Email").HasMaxLength(255).IsRequired();
            entity.Property(c => c.Intern).HasColumnName("Intern").IsRequired();
            entity.Property(c => c.Name).HasColumnName("Name").HasMaxLength(150).IsRequired();
            entity.Property(c => c.Surname).HasColumnName("Surname").HasMaxLength(150).IsRequired();
            entity.Property(c => c.Phone).HasColumnName("Phone").HasMaxLength(50).IsRequired();
            entity.Property(c => c.AvailabilityDate).HasColumnName("AvailabilityDate").IsRequired();
            entity.Property(c => c.ExpirationDateCI).HasColumnName("ExpirationDateCI").IsRequired();

            entity.HasMany(c => c.Profiles)
                .WithOne(p => p.Consultant)
                .HasForeignKey(p => p.ConsultantUuid)
                .OnDelete(DeleteBehavior.Cascade);

        });

        // Configuration de Profile
        modelBuilder.Entity<Profile>(entity =>
        {
            entity.ToTable("Profile");
            entity.HasKey(p => p.ProfileUuid);
            entity.Property(p => p.ProfileUuid).HasColumnName("ProfileUuid").IsRequired();
            entity.Property(p => p.Ratehour).HasColumnName("Ratehour").IsRequired();
            entity.Property(p => p.CV).HasColumnName("CV").HasMaxLength(500);
            entity.Property(p => p.CVDate).HasColumnName("CVDate").IsRequired();
            entity.Property(p => p.JobTitle).HasColumnName("JobTitle").HasMaxLength(150).IsRequired();
            entity.Property(p => p.ExperienceLevel)
                .HasConversion(x => x.ToString(), x => (Experience)Enum.Parse(typeof(Experience), x));


        });

        modelBuilder.Entity<Profile>()
        .Property(p => p.Skills)
        .HasConversion(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>()
        );

    modelBuilder.Entity<Profile>()
        .Property(p => p.Keywords)
        .HasConversion(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>()
        );

        modelBuilder.Entity<Keyword>(entity =>
        {
            entity.ToTable("Keyword");
            entity.HasKey(k => k.KeywordUuid);
            entity.Property(k => k.KeywordUuid).HasColumnName("KeywordUuid").IsRequired();
            entity.Property(k => k.Name).HasColumnName("Name").HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.ToTable("Skill");
            entity.HasKey(s => s.SkillUuid);
            entity.Property(s => s.SkillUuid).HasColumnName("SkillUuid").IsRequired();
            entity.Property(s => s.Name).HasColumnName("Name").HasMaxLength(255).IsRequired();
        });


        // Configuration du matching
        modelBuilder.Entity<Matching>(entity =>
        {
            entity.ToTable("Matching");
            entity.HasKey(m => m.MatchingUuid);
            entity.Property(m => m.MatchingUuid).HasColumnName("MatchingUuid").IsRequired();
            entity.Property(m => m.Comment).HasColumnName("Comment").HasMaxLength(500);
            entity.Property(m => m.Score).HasColumnName("Score").IsRequired();
            entity.Property(m => m.StatutMatching)
                .HasConversion(mx => mx.ToString(), mx => (StatutMatching)Enum.Parse(typeof(StatutMatching), mx));


            entity.HasOne(m => m.Profile)
                .WithMany()
                .HasForeignKey(m => m.ProfileUuid)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Rfp)
                .WithMany()
                .HasForeignKey(m => m.RfpUuid)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(m => new { m.RfpUuid, m.ProfileUuid }).IsUnique();
        });



    } 
}

