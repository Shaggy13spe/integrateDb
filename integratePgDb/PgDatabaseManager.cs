using integrateDb.Managers;

using Npgsql;

namespace integrateDb.PostgresSql {
    public class PgDatabaseManager : AdoDatabaseManager<PgDatabaseClient, NpgsqlConnection, NpgsqlCommand> {
        public static PgDatabaseManager Initialize(DatabaseManagerConfig config, params string[] fileNames) {
            var result = new PgDatabaseManager(config);
            foreach(var fileName in fileNames) {
                result.Load(fileName);
            }

            result.BeginTest();
            return result;
        }

        public static PgDatabaseManager Initialize(params string[] fileNames) {
            return Initialize(DatabaseManagerConfig.GlobalConfig, fileNames);
        }

        private PgDatabaseManager(DatabaseManagerConfig config)
            : base(config, new PgDatabaseClient(config.ReuseConnection)) { }
    }
}
