using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Allowed.EntityFrameworkCore.TempTables.PostgreSql.Databases
{
    public static class DatabaseExtensions
    {
        public static async Task CreateTempTableAsync<T>(this DbContext db, string tableName)
            where T : class
        {
            await db.Database.ExecuteSqlRawAsync(ScriptGenerator.GetCreate<T>(tableName));
        }

        public static async Task AddAsync<T>(this DbContext db, string tableName, T entity)
            where T : class
        {
            await db.Database.ExecuteSqlRawAsync(ScriptGenerator.GetInsert(tableName, new List<T> { entity }));
        }

        public static async Task AddRangeAsync<T>(this DbContext db, string tableName, List<T> entities)
            where T : class
        {
            await db.Database.ExecuteSqlRawAsync(ScriptGenerator.GetInsert(tableName, entities));
        }

        public static IQueryable<T> TempTable<T>(this DbContext db, string tableName)
            where T : class
        {
            return db.Set<T>().FromSqlRaw($"SELECT * FROM \"{tableName}\"");
        }
    }
}
