using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace Civitta.Task1.Api.Models
{
    public class DatabaseContext : DbContext
    {

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<CountryRegion> CountryRegions { get; set; }
        public DbSet<DayStatus> DayStatuses { get; set; }
        public DbSet<MaximumFreeDayInRow> MaximumFreeDayInRows { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code)
                      .IsRequired()
                      .HasMaxLength(3); 
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100); 
                entity.Property(e => e.DataFromDate)
                      .IsRequired();
                entity.Property(e => e.DataToDate)
                      .IsRequired();

                entity.HasMany(e => e.Regions)
                      .WithOne(e => e.Country)
                      .HasForeignKey(e => e.CountryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CountryRegion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code)
                      .IsRequired()
                      .HasMaxLength(10); 

                entity.HasOne(e => e.Country)
                      .WithMany(c => c.Regions)
                      .HasForeignKey(e => e.CountryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DayStatus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CountryCode)
                      .IsRequired()
                      .HasMaxLength(3);
                entity.Property(e => e.Region)
                      .HasMaxLength(10);
                entity.Property(e => e.Date)
                      .IsRequired();
                entity.Property(e => e.IsWorkDay)
                      .IsRequired();
                entity.Property(e => e.IsHoliday)
                      .IsRequired();
            });

            modelBuilder.Entity<Holiday>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.Date)
                      .IsRequired();

                entity.HasOne(e => e.Country)
                      .WithMany()
                      .HasForeignKey(e => e.CountryId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CountryRegion)
                      .WithMany()
                      .HasForeignKey(e => e.CountryRegionId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<MaximumFreeDayInRow>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Region)
                      .HasMaxLength(10); 
                entity.Property(e => e.CountryCode)
                      .IsRequired()
                      .HasMaxLength(3); 
                entity.Property(e => e.Year)
                      .IsRequired();
                entity.Property(e => e.MaximumFreeDayInRowCount)
                      .IsRequired();
            });
        }
    }
}
