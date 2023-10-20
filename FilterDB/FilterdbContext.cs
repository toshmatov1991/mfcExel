using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace exel_for_mfc;

public partial class FilterdbContext : DbContext
{
    public FilterdbContext()
    {
    }

    public FilterdbContext(DbContextOptions<FilterdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AreaF> AreaFs { get; set; }

    public virtual DbSet<LocalF> Localves { get; set; }

    public virtual DbSet<PayF> PayFs { get; set; }

    public virtual DbSet<PrivF> PrivFs { get; set; }

    public virtual DbSet<SolF> Solves { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite(PacHt());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AreaF>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AreaF");

            entity.Property(e => e.Flag).HasColumnName("flag");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<LocalF>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("LocalF");

            entity.Property(e => e.Flag).HasColumnName("flag");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<PayF>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("PayF");

            entity.Property(e => e.Flag).HasColumnName("flag");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<PrivF>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("PrivF");

            entity.Property(e => e.Flag).HasColumnName("flag");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<SolF>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SolF");

            entity.Property(e => e.Flag).HasColumnName("flag");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);

    }


    //Относительный путь
    static private string PacHt()
    {
        var x = Directory.GetCurrentDirectory();
        var y = Directory.GetParent(x).FullName;
        var c = Directory.GetParent(y).FullName;
        var r = "Data Source=" + Directory.GetParent(c).FullName + @"\FilterDB\filterdb.db";
        return r;
    }



    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
