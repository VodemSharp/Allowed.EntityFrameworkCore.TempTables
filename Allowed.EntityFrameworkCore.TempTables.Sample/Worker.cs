using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Databases;
using Allowed.EntityFrameworkCore.TempTables.Sample.Data;
using Allowed.EntityFrameworkCore.TempTables.Sample.Data.DbModels;
using Allowed.EntityFrameworkCore.TempTables.Sample.Data.Seeders;
using Allowed.EntityFrameworkCore.TempTables.Sample.Data.TempTables;
using Allowed.EntityFrameworkCore.TempTables.Sample.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Allowed.EntityFrameworkCore.TempTables.Sample
{
    public class Worker : BackgroundService
    {
        private TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(10);
        private Timer Timer { get; set; }

        private readonly IServiceProvider _services;
        private readonly ILogger<Worker> _logger;

        public Worker(IServiceProvider services, ILogger<Worker> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected async void DoWork(object state)
        {
            try
            {
                using IServiceScope scope = _services.CreateScope();
                ApplicationDbContext db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                await db.Seed();

                using IDbContextTransaction transaction = db.Database.BeginTransaction();

                // Not working
                List<Address> requestAddresses = new()
                {
                    new Address
                    {
                        Address1 = "Test Address",
                        City = "Test City",
                        PostalCode = "10000",
                        State = "Test State",
                        Latitude = 30,
                        Longitude = 40,
                        Type = AddressType.Physical
                    },
                    new Address
                    {
                        Address1 = "1745 T Street Southeast",
                        City = "Washington",
                        PostalCode = "20020",
                        State = "DC",
                        Latitude = 38.867033,
                        Longitude = -76.979235,
                        Type = AddressType.Physical
                    }
                };

                try
                {
                    List<Address> result = new();
                    foreach (Address address in requestAddresses.Where(ra => !db.Addresses.Any(a =>
                        ra.Latitude == a.Latitude && ra.Longitude == a.Longitude)))
                    {
                        result.Add(address);
                    }

                    //List<Address> result = await db.Addresses.Where(a => !requestAddresses.Any(
                    //    ra => ra.Latitude == a.Latitude && ra.Longitude == a.Longitude)).ToListAsync();
                }
                catch (Exception e)
                {

                }

                //await db.CreateTempTableAsync<TempAddress>("temp");
                //await db.AddRangeAsync<TempAddress>("temp", new List<TempAddress>
                //{
                //    new TempAddress
                //    {
                //        Address1 = "test",
                //        City = "Tokio",
                //        PostalCode = "code",
                //        State = "O'Kraken",
                //        Latitude = 10,
                //        Longitude = 20,
                //        Type = AddressType.Physical
                //    },
                //    new TempAddress
                //    {
                //        Address1 = "keks",
                //        City = "Kyiv",
                //        PostalCode = "another code",
                //        State = "O\"Pirate",
                //        Latitude = 30,
                //        Longitude = 40,
                //        Type = AddressType.Virtual
                //    }
                //});

                //List<TempAddress> result = await db.TempTable<TempAddress>("temp").ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Timer = new Timer(DoWork, null, TimeSpan.Zero, Interval);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            Timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            Timer?.Dispose();
        }
    }
}
