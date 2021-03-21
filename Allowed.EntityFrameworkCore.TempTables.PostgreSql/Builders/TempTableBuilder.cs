using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Configurations;
using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Enums;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Allowed.EntityFrameworkCore.TempTables.PostgreSql.Builders
{
    public class TempTableBuilder<T>
    {
        private TempTableBuilder<T> AddConfiguration(Expression<Func<T, object>> expression, ColumnConfigurationTypes type)
        {
            DatabaseConfiguration dbConfiguration = DatabaseConfiguration.GetInstance();
            TempTableConfiguration config = dbConfiguration.TempTableConfigurations.First(c => c.Type == typeof(T));

            ColumnsConfiguration columnConfig = new()
            {
                Type = type
            };

            Type bodyType = expression.Body.GetType();

            if (bodyType == typeof(UnaryExpression))
                columnConfig.Names.Add(((MemberExpression)((UnaryExpression)expression.Body).Operand).Member.Name);
            else if (bodyType.BaseType == typeof(MemberExpression))
                columnConfig.Names.Add(((MemberExpression)expression.Body).Member.Name);
            else if (bodyType == typeof(NewExpression))
                columnConfig.Names.AddRange(((NewExpression)expression.Body).Members.Select(x => x.Name));
            else
                throw new NotImplementedException(bodyType.Name);

            config.ColumnConfigurations.Add(columnConfig);
            return this;
        }

        public TempTableBuilder<T> HasIndex(Expression<Func<T, object>> expression)
        {
            return AddConfiguration(expression, ColumnConfigurationTypes.Index);
        }
    }
}
