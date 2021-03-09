using System;

namespace Allowed.EntityFrameworkCore.TempTables.PostgreSql.Entities
{
    public class EntityColumn
    {
        public string Name { get; set; }
        public string DbType { get; set; }
        public Type Type { get; set; }
    }
}
