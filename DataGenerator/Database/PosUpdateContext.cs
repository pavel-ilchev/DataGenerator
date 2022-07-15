using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DataGenerator.Database.PosUpdateEntities;

namespace DataGenerator.Database
{
    public partial class PosUpdateContext : DbContext
    {
        public PosUpdateContext()
        {
        }

        public PosUpdateContext(DbContextOptions<PosUpdateContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Client> Clients { get; set; } = null!;
        public virtual DbSet<Location> Locations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.ClientId).ValueGeneratedNever();

                entity.Property(e => e.RowId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.Property(e => e.LocationId).ValueGeneratedNever();

                entity.Property(e => e.ConnectionStringComponents).IsFixedLength();

                entity.Property(e => e.DeleteOldHashOn).IsFixedLength();

                entity.Property(e => e.RowId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Locations)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("FK_Locations_Client");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
