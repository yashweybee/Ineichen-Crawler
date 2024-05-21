using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Ineichen_Crawler.Models;

public partial class IneichenContext : DbContext
{
    public IneichenContext()
    {
    }

    public IneichenContext(DbContextOptions<IneichenContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Auction> Auctions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer("Server=DESKTOP-AH3AP4P\\MSSQLSERVER01;Database=Ineichen;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auction>(entity =>
        {
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.StartDate).HasColumnType("date");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
