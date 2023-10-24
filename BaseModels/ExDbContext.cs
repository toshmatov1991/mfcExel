using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Spreadsheet;
using exel_for_mfc;
using Microsoft.EntityFrameworkCore;

namespace exel_for_mfc;

public partial class ExDbContext : DbContext
{
    public ExDbContext()
    {
    }

    public ExDbContext(DbContextOptions<ExDbContext> options)
    : base(options)
    {
    }

    public virtual DbSet<Applicant> Applicants { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<Locality> Localities { get; set; }

    public virtual DbSet<PayAmount> PayAmounts { get; set; }

    public virtual DbSet<Privilege> Privileges { get; set; }

    public virtual DbSet<Registry> Registries { get; set; }

    public virtual DbSet<SolutionType> SolutionTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Database=ExDB;Trusted_connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Applicant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Applican__3214EC07D69244C5");

            entity.ToTable("Applicant");

            entity.Property(e => e.Adress).HasMaxLength(250);
            entity.Property(e => e.AreaFk).HasColumnName("Area_FK");
            entity.Property(e => e.Firstname).HasMaxLength(120);
            entity.Property(e => e.Lastname).HasMaxLength(120);
            entity.Property(e => e.LocalityFk).HasColumnName("Locality_FK");
            entity.Property(e => e.Middlename).HasMaxLength(120);
            entity.Property(e => e.PrivilegesFk).HasColumnName("Privileges_FK");
            entity.Property(e => e.Snils).HasMaxLength(20);

            entity.HasOne(d => d.AreaFkNavigation).WithMany(p => p.Applicants)
                .HasForeignKey(d => d.AreaFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Applicant__Area___2A4B4B5E");

            entity.HasOne(d => d.LocalityFkNavigation).WithMany(p => p.Applicants)
                .HasForeignKey(d => d.LocalityFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Applicant__Local__2B3F6F97");

            entity.HasOne(d => d.PrivilegesFkNavigation).WithMany(p => p.Applicants)
                .HasForeignKey(d => d.PrivilegesFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Applicant__Privi__2C3393D0");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Area__3214EC07BE6CF96C");

            entity.ToTable("Area");

            entity.Property(e => e.AreaName).HasMaxLength(120);

            entity.Property(e => e.HidingArea);

        });

        modelBuilder.Entity<Locality>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Locality__3214EC07332A7940");

            entity.ToTable("Locality");

            entity.Property(e => e.LocalName).HasMaxLength(100);

            entity.Property(e => e.HidingLocal);

        });

        modelBuilder.Entity<PayAmount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PayAmoun__3214EC079E500CA6");

            entity.ToTable("PayAmount");

            entity.Property(e => e.Pay).HasColumnType("decimal(18, 0)");

            entity.Property(e => e.HidingPay);

            entity.Property(e => e.Mkr).HasMaxLength(30);

            entity.Property(e => e.Ulica).HasMaxLength(30);

            entity.Property(e => e.Numbedom).HasMaxLength(30);

            entity.Property(e => e.Stroenie).HasMaxLength(30);

            entity.Property(e => e.Kvartira).HasMaxLength(30);

        });

        modelBuilder.Entity<Privilege>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Privileg__3214EC07CE9AC58C");

            entity.Property(e => e.PrivilegesName).HasMaxLength(100);

            entity.Property(e => e.HidingPriv);

        });

        modelBuilder.Entity<Registry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Registry__3214EC07BF59E2D2");

            entity.ToTable("Registry");

            entity.Property(e => e.ApplicantFk).HasColumnName("Applicant_FK");
            entity.Property(e => e.Comment).HasMaxLength(450);
            entity.Property(e => e.DateAndNumbSolutionSert).HasMaxLength(100);
            entity.Property(e => e.DateGetSert).HasColumnType("date");
            entity.Property(e => e.MailingDate).HasColumnType("date");
            entity.Property(e => e.PayAmountFk).HasColumnName("PayAmount_FK");
            entity.Property(e => e.SerialAndNumberSert).HasMaxLength(150);
            entity.Property(e => e.SolutionFk).HasColumnName("Solution_FK");
            entity.Property(e => e.Trek).HasMaxLength(120);

            entity.HasOne(d => d.ApplicantFkNavigation).WithMany(p => p.Registries)
                .HasForeignKey(d => d.ApplicantFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Registry__Applic__33D4B598");

            entity.HasOne(d => d.PayAmountFkNavigation).WithMany(p => p.Registries)
                .HasForeignKey(d => d.PayAmountFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Registry__PayAmo__34C8D9D1");

            entity.HasOne(d => d.SolutionFkNavigation).WithMany(p => p.Registries)
                .HasForeignKey(d => d.SolutionFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Registry__Soluti__35BCFE0A");
        });

        modelBuilder.Entity<SolutionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Solution__3214EC07CB71BCC4");

            entity.ToTable("SolutionType");

            entity.Property(e => e.SolutionName).HasMaxLength(50);

            entity.Property(e => e.Login).HasMaxLength(30);

            entity.Property(e => e.Passwords).HasMaxLength(30);

            entity.Property(e => e.Rolle).HasMaxLength(30);

            entity.Property(e => e.HidingSol);

        });

        OnModelCreatingPartial(modelBuilder); 
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
