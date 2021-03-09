using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Configurations;
using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Allowed.EntityFrameworkCore.TempTables.PostgreSql
{
    public static class ScriptGenerator
    {
        private static bool IsNullable(Type type)
        {
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        private static List<EntityColumn> ListColumns<T>()
        {
            List<EntityColumn> columns = new List<EntityColumn> { };
            foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                    .Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any()))
            {
                string type;
                List<ColumnAttribute> columnAttributes = property.GetCustomAttributes<ColumnAttribute>().ToList();
                if (columnAttributes.Any(a => !string.IsNullOrEmpty(a.TypeName)))
                    type = columnAttributes.First(a => !string.IsNullOrEmpty(a.TypeName)).TypeName;
                else
                    type = ColumnTypeHelper.GetDbType(property.PropertyType);

                if (string.IsNullOrEmpty(type))
                    continue;

                columns.Add(new EntityColumn
                {
                    Name = property.Name,
                    DbType = type,
                    Type = property.PropertyType
                });
            }

            return columns;
        }

        public static string GetCreate<T>(string tableName)
        {
            StringBuilder builder = new StringBuilder($"CREATE TEMP TABLE \"{tableName}\" (");

            int i = 0;
            foreach (EntityColumn column in ListColumns<T>())
            {
                if (i != 0)
                    builder.Append(", ");

                builder.Append($"\n\"{column.Name}\" {column.DbType}");

                if (!IsNullable(column.Type))
                    builder.Append(" NOT NULL");

                i++;
            }

            builder.Append("\n);");

            DatabaseConfiguration dbConfig = DatabaseConfiguration.GetInstance();
            TempTableConfiguration config = dbConfig.TempTableConfigurations.FirstOrDefault();

            if (config != null)
            {
                foreach (ColumnConfiguration columnConfig in config.ColumnConfigurations)
                {
                    if (columnConfig.Type == Enums.ColumnConfigurationTypes.Index)
                        builder.Append($"\nCREATE INDEX \"IX_{tableName}_{columnConfig.Name}\" ON \"{tableName}\" (\"{columnConfig.Name}\");");
                }
            }

            return builder.ToString();
        }

        public static async Task<string> GetInsert<T>(string tableName, List<T> entities)
        {
            StringBuilder builder = new StringBuilder($"INSERT INTO \"{tableName}\" (");
            List<EntityColumn> columns = ListColumns<T>();

            int i = 0;
            foreach (EntityColumn column in columns)
            {
                if (i != 0)
                    builder.Append(", ");

                builder.Append($"\"{column.Name}\"");
                i++;
            }

            builder.Append(") VALUES\n");

            i = 0;
            foreach (T entity in entities)
            {
                if (i == 0)
                    builder.Append("(");
                else
                    builder.Append(",\n(");

                int j = 0;
                foreach (EntityColumn column in columns)
                {
                    if (j != 0)
                        builder.Append(", ");

                    builder.Append(ColumnTypeHelper.GetForQuery(column.Type, typeof(T).GetProperty(column.Name).GetValue(entity)?.ToString()));
                    j++;
                }

                builder.Append(')');
                i++;
            }

            return builder.ToString();
        }
    }
}
