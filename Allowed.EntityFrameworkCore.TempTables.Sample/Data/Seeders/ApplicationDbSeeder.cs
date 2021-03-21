using Allowed.EntityFrameworkCore.TempTables.Sample.Data.DbModels;
using Allowed.EntityFrameworkCore.TempTables.Sample.Enums;
using Allowed.EntityFrameworkCore.TempTables.Sample.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Allowed.EntityFrameworkCore.TempTables.Sample.Data.Seeders
{
    public static class ApplicationDbSeeder
    {
        public static async Task<ApplicationDbContext> Seed(this ApplicationDbContext db)
        {
            string json = await File.ReadAllTextAsync("addresses.json");
            List<JsonAddress> addresses = JsonConvert.DeserializeObject<List<JsonAddress>>(json);

            foreach (JsonAddress address in addresses)
            {
                if (!await db.Addresses.AnyAsync(a => a.Latitude == address.Coordinates.Latitude && a.Longitude == address.Coordinates.Longitude))
                {
                    await db.Addresses.AddAsync(new Address
                    {
                        Address1 = address.Address1,
                        Address2 = address.Address2,
                        City = address.City,
                        Latitude = address.Coordinates.Latitude,
                        Longitude = address.Coordinates.Longitude,
                        PostalCode = address.PostalCode,
                        State = address.State,
                        Type = AddressType.Virtual
                    });
                }
            }

            await db.SaveChangesAsync();

            return db;
        }
    }
}
