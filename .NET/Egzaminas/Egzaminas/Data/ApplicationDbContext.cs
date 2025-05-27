using System;
using Egzaminas.Models;
using Microsoft.EntityFrameworkCore;

namespace Egzaminas.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<UserInfo> Users { get; set; }
    public DbSet<PersonInfo> People { get; set; }
    public DbSet<AddressInfo> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserInfo>()
            .HasOne(u => u.PersonInfo)
            .WithOne(p => p.User)
            .HasForeignKey<PersonInfo>(p => p.Id);

        modelBuilder.Entity<PersonInfo>()
            .HasOne(p => p.Address)
            .WithMany(a => a.People)
            .HasForeignKey(p => p.AddressInfoId);

        modelBuilder.Entity<UserInfo>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        modelBuilder.Entity<PersonInfo>()
            .HasIndex(p => p.PersonalNumber)
            .IsUnique();
    }
}
