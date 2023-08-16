using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace exel_for_mfc.Models;

public partial class ExelDbContext : DbContext
{
    public ExelDbContext()
    {
    }

    public ExelDbContext(DbContextOptions<ExelDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Adress> Adresses { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<CertificateSolution> CertificateSolutions { get; set; }

    public virtual DbSet<Fio> Fios { get; set; }

    public virtual DbSet<Locality> Localities { get; set; }

    public virtual DbSet<PayAmount> PayAmounts { get; set; }

    public virtual DbSet<Privilege> Privileges { get; set; }

    public virtual DbSet<Registry> Registries { get; set; }

    public virtual DbSet<StatusEx> Statusexes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=192.168.155.170;Initial Catalog=ExelDB;Persist Security Info=True;TrustServerCertificate=True;User ID=SA;Password=QWEasd123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Adress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Adress__3214EC07C253EE32");

            entity.ToTable("Adress");

            entity.Property(e => e.DomOrStr).HasMaxLength(10);
            entity.Property(e => e.NameStreet).HasMaxLength(150);
            entity.Property(e => e.UlMkr)
                .HasMaxLength(15)
                .HasColumnName("Ul_Mkr");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Area__3214EC072DC7B601");

            entity.ToTable("Area");

            entity.Property(e => e.AreaName).HasMaxLength(120);
        });

        modelBuilder.Entity<CertificateSolution>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Certific__3214EC07FFE0CECE");

            entity.ToTable("CertificateSolution");

            entity.Property(e => e.DateDecision).HasColumnType("date");
            entity.Property(e => e.NumberDecision).HasMaxLength(110);
        });

        modelBuilder.Entity<Fio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FIO__3214EC07E78AB2C4");

            entity.ToTable("FIO");

            entity.Property(e => e.Firstname).HasMaxLength(100);
            entity.Property(e => e.Lastname).HasMaxLength(100);
            entity.Property(e => e.Middlename).HasMaxLength(100);
        });

        modelBuilder.Entity<Locality>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Locality__3214EC079173E09E");

            entity.ToTable("Locality");

            entity.Property(e => e.LocalName).HasMaxLength(150);
        });

        modelBuilder.Entity<PayAmount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PayAmoun__3214EC0706B39D51");

            entity.ToTable("PayAmount");

            entity.Property(e => e.Pay).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<Privilege>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Privileg__3214EC0757D2C199");

            entity.Property(e => e.PrivilegesName).HasMaxLength(150);
        });

        modelBuilder.Entity<Registry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Registry__3214EC07A725B65D");

            entity.ToTable("Registry");

            entity.Property(e => e.AddressFk).HasColumnName("AddressFK");
            entity.Property(e => e.ApplicantFk).HasColumnName("ApplicantFK");
            entity.Property(e => e.AreaFk).HasColumnName("AreaFK");
            entity.Property(e => e.CertificateSolutionFk).HasColumnName("CertificateSolutionFK");
            entity.Property(e => e.LocalityFk).HasColumnName("LocalityFK");
            entity.Property(e => e.MailingDate).HasColumnType("date");
            entity.Property(e => e.PaymentAmountFk).HasColumnName("PaymentAmountFK");
            entity.Property(e => e.PrivilegesFk).HasColumnName("PrivilegesFK");
            entity.Property(e => e.SerialNumberOldSert).HasMaxLength(120);
            entity.Property(e => e.Snils).HasMaxLength(20);
            entity.Property(e => e.StatusSertFk).HasColumnName("StatusSertFK");
            entity.Property(e => e.Trek).HasMaxLength(120);

            entity.HasOne(d => d.AddressFkNavigation).WithMany(p => p.Registries)
                .HasForeignKey(d => d.AddressFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Registry__Addres__5441852A");

            entity.HasOne(d => d.ApplicantFkNavigation).WithMany(p => p.Registries)
                .HasForeignKey(d => d.ApplicantFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Registry__Applic__5165187F");

            entity.HasOne(d => d.AreaFkNavigation).WithMany(p => p.Registries)
                .HasForeignKey(d => d.AreaFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Registry__AreaFK__52593CB8");

            entity.HasOne(d => d.CertificateSolutionFkNavigation).WithMany(p => p.Registries)
                .HasForeignKey(d => d.CertificateSolutionFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Registry__Certif__5812160E");

            entity.HasOne(d => d.LocalityFkNavigation).WithMany(p => p.Registries)
                .HasForeignKey(d => d.LocalityFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Registry__Locali__534D60F1");

            entity.HasOne(d => d.PaymentAmountFkNavigation).WithMany(p => p.Registries)
                .HasForeignKey(d => d.PaymentAmountFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Registry__Paymen__5629CD9C");

            entity.HasOne(d => d.PrivilegesFkNavigation).WithMany(p => p.Registries)
                .HasForeignKey(d => d.PrivilegesFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Registry__Privil__5535A963");

            entity.HasOne(d => d.StatusSertFkNavigation).WithMany(p => p.Registries)
                .HasForeignKey(d => d.StatusSertFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Registry__Status__571DF1D5");
        });

        modelBuilder.Entity<StatusEx>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StatusEx__3214EC079B33E9C2");

            entity.ToTable("StatusEx");

            entity.Property(e => e.StatusName).HasMaxLength(150);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
