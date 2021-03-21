using Allowed.EntityFrameworkCore.TempTables.PostgreSql.Enums;
using System.Collections.Generic;

namespace Allowed.EntityFrameworkCore.TempTables.PostgreSql.Configurations
{
    public class ColumnsConfiguration
    {
        public List<string> Names { get; set; } = new List<string> { };
        public ColumnConfigurationTypes Type { get; set; }
    }
}
