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

    }
}
