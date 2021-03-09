using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Configurations;
using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Enums;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Allowed.EntityFrameworkCore.TempTables.PostgreSql.Builders
{
    public class TempTableBuilder<T>
    {
        public void HasIndex(Expression<Func<T, object>> expression)
        {
            DatabaseConfiguration dbConfiguration = DatabaseConfiguration.GetInstance();
            TempTableConfiguration config = dbConfiguration.TempTableConfigurations.First(c => c.Type == typeof(T));

            config.ColumnConfigurations.Add(new ColumnConfiguration
            {
                Type = ColumnConfigurationTypes.Index,
                Name = ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member.Name
            });
        }
    }
}
