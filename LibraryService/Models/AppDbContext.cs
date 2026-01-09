#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
    }
    public virtual DbSet<MyLibrary> MyLibrary { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MyLibrary>(entity =>
        {
            entity.ToTable("mylibrary", "libraryservice");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.User_id)
                .HasColumnType("integer")
                .HasColumnName("user_id");
            entity.Property(e => e.Book_id)
                .HasColumnType("integer")
                .HasColumnName("book_id");
            entity.Property(e => e.Status)
                .HasColumnType("character varying")
                .HasColumnName("status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}