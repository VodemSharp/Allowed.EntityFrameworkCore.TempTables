using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Configurations;
using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Entities;
using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

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
            List<EntityColumn> columns = new() { };
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
            DatabaseConfiguration dbConfig = DatabaseConfiguration.GetInstance();
            TempTableConfiguration config = dbConfig.TempTableConfigurations.First();

            StringBuilder builder = new($"CREATE TEMP TABLE \"{tableName}\" (");

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

            if (config != null)
            {
                foreach (ColumnsConfiguration columnConfig in config.ColumnConfigurations)
                {
                    if (columnConfig.Type == ColumnConfigurationTypes.Index)
                        builder.Append($"\nCREATE INDEX \"IX_{tableName}_{string.Join('_', columnConfig.Names)}\" ON \"{tableName}\" (\"{string.Join("\", \"", columnConfig.Names)}\");");
                }
            }

            return builder.ToString();
        }

        private static string PrepareValue<T>(T entity, string columnName)
        {
            PropertyInfo propertyInfo = typeof(T).GetProperty(columnName);
            object value = propertyInfo.GetValue(entity);

            if (propertyInfo.PropertyType.IsEnum)
                return ((int)Enum.Parse(propertyInfo.PropertyType, value?.ToString())).ToString();
            else
                return value?.ToString().Replace("'", "''");
        }

        public static string GetInsert<T>(string tableName, List<T> entities)
        {
            StringBuilder builder = new($"INSERT INTO \"{tableName}\" (");
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
                    builder.Append('(');
                else
                    builder.Append(",\n(");

                int j = 0;
                foreach (EntityColumn column in columns)
                {
                    if (j != 0)
                        builder.Append(", ");

                    builder.Append(ColumnTypeHelper.GetForQuery(column.Type, PrepareValue(entity, column.Name)));

                    j++;
                }

                builder.Append(')');
                i++;
            }

            return builder.ToString();
        }
    }
}
