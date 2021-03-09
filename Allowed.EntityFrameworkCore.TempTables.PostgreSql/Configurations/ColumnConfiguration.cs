using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Enums;

namespace Allowed.EntityFrameworkCore.TempTables.PostgreSql.Configurations
{
    public class ColumnConfiguration
    {
        public string Name { get; set; }
        public ColumnConfigurationTypes Type { get; set; }
    }
}
