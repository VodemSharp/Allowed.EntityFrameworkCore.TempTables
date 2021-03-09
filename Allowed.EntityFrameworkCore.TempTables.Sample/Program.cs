using Allowed.EntityFrameworkCore.TempTables.PostgreSql;
using Allowed.EntityFrameworkCore.TempTables.Sample.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.EntityFrameworkCore.TempTables.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration config = hostContext.Configuration;

                    string connection = config.GetConnectionString("DefaultConnection");
                    services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connection));

                    services.AddHostedService<Worker>();
                });
    }
}