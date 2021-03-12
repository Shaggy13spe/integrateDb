using Microsoft.Data.SqlClient;

using integrateDb.Managers;

namespace integrateDb.SqlServer {
    public class SqlDatabaseManager : AdoDatabaseManager<SqlDatabaseClient, SqlConnection, SqlCommand> {
        public static SqlDatabaseManager Initialize(DatabaseManagerConfig config, params string[] fileNames) {
            var result = new SqlDatabaseManager(config);
            foreach(var fileName in fileNames) {
                result.Load(fileName);
            }

            result.BeginTest();
            return result;
        }

        public static SqlDatabaseManager Initialize(params string[] fileNames) {
            return Initialize(DatabaseManagerConfig.GlobalConfig, fileNames);
        }

        private SqlDatabaseManager(DatabaseManagerConfig config)
            : base(config, new SqlDatabaseClient(config.ReuseConnection)) { }
    }
}
