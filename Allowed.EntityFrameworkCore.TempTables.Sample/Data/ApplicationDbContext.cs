using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Extensions;
using Allowed.EntityFrameworkCore.TempTables.Sample.Data.DbModels;
using Allowed.EntityFrameworkCore.TempTables.Sample.Data.TempTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace Allowed.EntityFrameworkCore.TempTables.Sample.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Address>().HasIndex(d => new { d.Latitude, d.Longitude });
            builder.TempTable<TempAddress>().HasIndex(d => new { d.Latitude, d.Longitude });
        }

        public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext(string[] args)
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

                string connectionString = config.GetConnectionString("DefaultConnection");
                builder.UseNpgsql(connectionString);

                return new ApplicationDbContext(builder.Options);
            }
        }
    }
}
