using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Databases;
using Allowed.EntityFrameworkCore.TempTables.Sample.Data;
using Allowed.EntityFrameworkCore.TempTables.Sample.Data.TempTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Allowed.EntityFrameworkCore.TempTables.Sample
{
    public class Worker : BackgroundService
    {
        private TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(15);
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
                //await db.Seed();

                using IDbContextTransaction transaction = db.Database.BeginTransaction();

                await db.CreateTempTableAsync<TempAddress>("temp");
                await db.AddAsync("temp", new TempAddress
                {
                    Address1 = "test",
                    City = "Tokio",
                    Latitude = 10,
                    Longitude = 20,
                    PostalCode = "code",
                    State = "Kraken"
                });

                List<TempAddress> result = await db.TempTable<TempAddress>("temp").ToListAsync();
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
