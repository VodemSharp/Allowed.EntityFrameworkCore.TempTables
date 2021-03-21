using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Builders;
using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Allowed.EntityFrameworkCore.TempTables.PostgreSql.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static TempTableBuilder<T> TempTable<T>(this ModelBuilder builder)
            where T : class
        {
            builder.Entity<T>().HasNoKey();
            builder.Entity<T>().ToView(null);

            DatabaseConfiguration.GetInstance().TempTableConfigurations.Add(new TempTableConfiguration(typeof(T)));

            return new TempTableBuilder<T>();
        }
    }
}
