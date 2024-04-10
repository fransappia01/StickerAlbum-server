using Microsoft.EntityFrameworkCore;
using StickerAlbum.Models;
using System;

namespace StickerAlbum.Context;

public class AlbumDbContext: DbContext
{
    public AlbumDbContext(DbContextOptions<AlbumDbContext> options) : base(options)
    {
        Console.WriteLine("Contexto de base de datos creado.");
    }
    // Tablas
    public DbSet<Users> Users { get; set; }
    public DbSet<Albums> Albums { get; set; }
    public DbSet<Stickers> Stickers { get; set; }
    public DbSet<Stickers_x_Albums> Stickers_x_Albums { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.UserID).HasName("PK__Users__1788CCAC37E88EB3");

            entity.Property(e => e.UserID)
                .ValueGeneratedOnAdd()
                .HasColumnName("UserID");

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("Email");

            entity.Property(e => e.FirstName)
               .HasMaxLength(255)
               .HasColumnName("FirstName");

            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("LastName");

            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("Password");

        });

        modelBuilder.Entity<Albums>(entity =>
        {
            entity.HasKey(a => a.AlbumID);

        });

        modelBuilder.Entity<Stickers>(entity =>
        {
            entity.HasKey(s => s.StickerID);

        });

        modelBuilder.Entity<Stickers_x_Albums>()
            .HasKey(sa => sa.StickerAlbumId);

        modelBuilder.Entity<Stickers_x_Albums>()
            .Property(sa => sa.Status)
            .IsRequired();
    }
}
