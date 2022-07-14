namespace DataGenerator.Database;

using CpEntities;
using Microsoft.EntityFrameworkCore;

public class CpContext : DbContext
{
    public CpContext()
    {
    }

    public CpContext(DbContextOptions<CpContext> options) : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; } = null!;
    public virtual DbSet<ClientLocation> ClientLocations { get; set; } = null!;
    public virtual DbSet<Location> Locations { get; set; } = null!;
    public virtual DbSet<LocationsInfo> LocationsInfos { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity => { entity.Property(e => e.PlatformVersion).HasDefaultValueSql("((1))"); });

        modelBuilder.Entity<ClientLocation>(entity =>
        {
            entity.HasOne(d => d.Client).WithMany(p => p.ClientLocations).HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ClientLocation_Client");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.Property(e => e.CallClassifierCutoff).HasDefaultValueSql("((0))");

            entity.Property(e => e.CallClassifierEnabled).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.Client).WithMany(p => p.Locations).HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_Locations_Client");
        });

        modelBuilder.Entity<LocationsInfo>(entity =>
        {
            entity.Property(e => e.DateAdded).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Client).WithMany(p => p.LocationsInfos).HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_LocationInfo_Clients");

            entity.HasOne(d => d.Location).WithOne(p => p.LocationsInfo).HasForeignKey<LocationsInfo>(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_LocationsInfo_Locations");
        });
    }
}