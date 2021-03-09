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
            string script = await ScriptGenerator.GetInsert(tableName, new List<T> { entity });
            await db.Database.ExecuteSqlRawAsync(script);
        }

        public static IQueryable<T> TempTable<T>(this DbContext db, string tableName)
            where T : class
        {
            return db.Set<T>().FromSqlRaw($"SELECT * FROM \"{tableName}\";");
        }
    }
}
