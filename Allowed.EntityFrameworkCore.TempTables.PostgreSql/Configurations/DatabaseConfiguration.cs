using System.Collections.Generic;

namespace Allowed.EntityFrameworkCore.TempTables.PostgreSql.Configurations
{
    public class DatabaseConfiguration
    {
        private static DatabaseConfiguration _instance;
        private readonly static object _locker = new object();

        public static DatabaseConfiguration GetInstance()
        {
            lock (_locker)
            {
                if (_instance == null)
                    _instance = new DatabaseConfiguration();
            }

            return _instance;
        }

        public List<TempTableConfiguration> TempTableConfigurations { get; set; } = new List<TempTableConfiguration> { };
    }
}
