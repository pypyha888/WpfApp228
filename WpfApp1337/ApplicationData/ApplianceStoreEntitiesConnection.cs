using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ApplianceStoreIS.ApplicationData
{
    public static class ApplianceStoreEntitiesConnection
    {
        public const string ConnectionName = "ApplianceStoreEntities";

        public static string GetConnectionString()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[ConnectionName]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("В App.config не найдена строка подключения 'ApplianceStoreEntities'.");
            }

            return connectionString;
        }

        public static string GetMasterConnectionString()
        {
            var builder = new SqlConnectionStringBuilder(GetConnectionString())
            {
                InitialCatalog = "master"
            };

            return builder.ConnectionString;
        }
    }
}
