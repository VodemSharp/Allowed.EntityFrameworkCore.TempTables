using NpgsqlTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Allowed.EntityFrameworkCore.TempTables.PostgreSql
{
    public static class ColumnTypeHelper
    {
        private static readonly Dictionary<Type, string> Types =
            new Dictionary<Type, string> {
                { typeof(bool), "boolean" },
                { typeof(byte), "smallint" },
                { typeof(sbyte), "smallint" },
                { typeof(short), "smallint" },
                { typeof(int), "integer" },
                { typeof(long), "bigint" },
                { typeof(float), "real" },
                { typeof(double), "double precision" },
                { typeof(decimal), "numeric" },
                { typeof(string), "text" },
                { typeof(char[]), "text" },
                { typeof(char), "text" },
                { typeof(NpgsqlPoint), "point" },
                { typeof(NpgsqlLSeg), "lseg" },
                { typeof(NpgsqlPath), "path" },
                { typeof(NpgsqlPolygon), "polygon" },
                { typeof(NpgsqlLine), "line" },
                { typeof(NpgsqlCircle), "circle" },
                { typeof(NpgsqlBox), "box" },
                { typeof(BitArray), "bit varying" },
                { typeof(Guid), "uuid" },
                { typeof(IPAddress), "inet" },
                { typeof(ValueTuple<IPAddress, int>), "inet" },
                { typeof(PhysicalAddress), "macaddr" },
                { typeof(NpgsqlTsQuery), "tsquery" },
                { typeof(NpgsqlTsVector), "tsvector" },
                { typeof(NpgsqlDate), "date" },
                { typeof(NpgsqlDateTime), "timestamp without time zone" },
                { typeof(DateTime), "timestamp without time zone" },
                { typeof(DateTimeOffset), "timestamp with time zone" },
                { typeof(TimeSpan), "interval" },
                { typeof(byte[]), "bytea" },
            };

        private static string AppendQuotes(string value)
        {
            return $"'{value}'";
        }

        private static readonly Dictionary<Type, Func<string, string>> Funcs =
            new Dictionary<Type, Func<string, string>> {
                { typeof(bool), null },
                { typeof(byte), null },
                { typeof(sbyte), null },
                { typeof(short), null },
                { typeof(int), null },
                { typeof(long), null },
                { typeof(float), null },
                { typeof(double), null },
                { typeof(decimal), null },
                { typeof(string), AppendQuotes },
                { typeof(char[]), AppendQuotes },
                { typeof(char), AppendQuotes },
                //{ typeof(NpgsqlPoint), "point" },
                //{ typeof(NpgsqlLSeg), "lseg" },
                //{ typeof(NpgsqlPath), "path" },
                //{ typeof(NpgsqlPolygon), "polygon" },
                //{ typeof(NpgsqlLine), "line" },
                //{ typeof(NpgsqlCircle), "circle" },
                //{ typeof(NpgsqlBox), "box" },
                //{ typeof(BitArray), "bit varying" },
                //{ typeof(Guid), "uuid" },
                //{ typeof(IPAddress), "inet" },
                //{ typeof(ValueTuple<IPAddress, int>), "inet" },
                //{ typeof(PhysicalAddress), "macaddr" },
                //{ typeof(NpgsqlTsQuery), "tsquery" },
                //{ typeof(NpgsqlTsVector), "tsvector" },
                //{ typeof(NpgsqlDate), "date" },
                //{ typeof(NpgsqlDateTime), "timestamp without time zone" },
                //{ typeof(DateTime), "timestamp without time zone" },
                //{ typeof(DateTimeOffset), "timestamp with time zone" },
                //{ typeof(TimeSpan), "interval" },
                //{ typeof(byte[]), "bytea" },
            };

        public static string GetDbType(Type columnType)
        {
            if (Types.ContainsKey(columnType))
                return Types[columnType];

            if (columnType.IsEnum)
                return "integer"; // "enum types";
            //else if (columnType.IsArray)
            //    return "array types";

            //columnType = columnType.IsGenericType ? columnType.GetGenericTypeDefinition() : columnType;
            //if (columnType == typeof(NpgsqlRange<>))
            //    return "range types";

            return null;
        }

        public static string GetForQuery(Type type, string value)
        {
            if (value == null)
                return "NULL";

            Func<string, string> prepareFunc = Funcs[type];

            if (prepareFunc != null)
                return prepareFunc.Invoke(value);

            return value;
        }
    }
}
