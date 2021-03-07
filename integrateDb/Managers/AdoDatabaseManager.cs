using System;
using System.Configuration;
using System.Data.Common;

using integrateDb.DatabaseClient;
using integrateDb.Interfaces;

namespace integrateDb.Managers {
    public abstract class AdoDatabaseManager<TAdoDatabaseClient, TDbConnection, TDbCommand> : DatabaseManager<TAdoDatabaseClient>
        where TAdoDatabaseClient : AdoDatabaseClient<TDbConnection, TDbCommand>, IDatabaseClient
        where TDbConnection : DbConnection
        where TDbCommand : DbCommand {

        protected readonly TAdoDatabaseClient databaseClient;

        public AdoDatabaseManager(DatabaseManagerConfig config, TAdoDatabaseClient databaseClient) : base(config) {
            this.databaseClient = databaseClient;
            DatabaseClient = this.databaseClient;
        }

        public AdoDatabaseManager<TAdoDatabaseClient, TDbConnection, TDbCommand> SetConnectionString(string connectionStringName) {
            var connectionStringDetail = ConfigurationManager.ConnectionStrings[connectionStringName];
            if(connectionStringDetail == null) {
                string message = $"Connection String '{connectionStringName}' not found";
                throw new Exception(message);
            }

            databaseClient.ConnectionString = connectionStringDetail.ConnectionString;
            return this;
        }

        public AdoDatabaseManager<TAdoDatabaseClient, TDbConnection, TDbCommand> PassConnectionString(string connectionstring) {
            databaseClient.ConnectionString = connectionstring;
            return this;
        }

        protected override void ValidateDependencies() {
            base.ValidateDependencies();
            if(string.IsNullOrWhiteSpace(databaseClient.ConnectionString))
                throw new InvalidOperationException("ConnectionString not specified");
        }
    }
}
