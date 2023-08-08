using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace exel_for_mfc.PassModels;

public partial class PassContext : DbContext
{
    public PassContext()
    {
    }

    public PassContext(DbContextOptions<PassContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Password> Passwords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite(PacHt());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Password>(entity =>
        {
            entity.HasIndex(e => e.Login, "IX_Passwords_login").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.Pass).HasColumnName("pass");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    //Относительный путь
    static private string PacHt()
    {
        var x = Directory.GetCurrentDirectory();
        var y = Directory.GetParent(x).FullName;
        var c = Directory.GetParent(y).FullName;
        var r = "Data Source=" + Directory.GetParent(c).FullName + @"\DA\pass.db";
        return r;
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}