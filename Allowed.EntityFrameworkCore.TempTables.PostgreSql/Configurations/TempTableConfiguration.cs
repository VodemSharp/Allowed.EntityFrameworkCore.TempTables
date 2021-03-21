using System;
using System.Collections.Generic;

namespace Allowed.EntityFrameworkCore.TempTables.PostgreSql.Configurations
{
    public class TempTableConfiguration
    {
        public Type Type { get; set; }
        public List<ColumnsConfiguration> ColumnConfigurations { get; set; } = new List<ColumnsConfiguration> { };

        public TempTableConfiguration(Type type)
        {
            Type = type;
        }
    }
}
