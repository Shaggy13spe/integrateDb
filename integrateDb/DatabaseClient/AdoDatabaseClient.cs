using System.Collections.Generic;
using System.Data.Common;

using integrateDb.Interfaces;
using integrateDb.Managers;
using integrateDb.Models;

namespace integrateDb.DatabaseClient {
    public abstract class AdoDatabaseClient<TDbConnection, TDbCommand> : IDatabaseClient
        where TDbConnection : DbConnection
        where TDbCommand : DbCommand {

        private readonly bool reuseConnection = false;
        private readonly Dictionary<string, TDbConnection> connections = new Dictionary<string, TDbConnection>();
        private readonly object syncLock = new object();

        public string ConnectionString { get; set; }

        public AdoDatabaseClient(bool reuseConnection) {
            this.reuseConnection = reuseConnection;
        }

        public void ExecuteCommand(string command) {
            ExecuteCommand(command, ConnectionString);
        }

        public abstract void InsertTableData(Dataset dataset);
        public abstract Dataset ReadTableData(string command, FormatterManager formatter);

        protected void ExecuteCommand(string command, string connectionString) {
            var connection = GetDbConnection(connectionString);
            try {
                connection.Open();
                using var dbCommand = CreateDbCommand(command, connection);
                dbCommand.ExecuteNonQuery();
            }
            finally {
                DisposeConnection(connection);
            }
        }

        protected abstract TDbConnection CreateDbConnection(string connectionString);
        protected abstract TDbCommand CreateDbCommand(string command, TDbConnection connection);

        private TDbConnection GetDbConnection(string connectionString) {
            if(!reuseConnection)
                return CreateDbConnection(connectionString);

            TDbConnection connection;
            lock(syncLock) {
                var connectionExists = connections.ContainsKey(connectionString);
                if(connectionExists)
                    connection = connections[connectionString];
                else {
                    connection = CreateDbConnection(connectionString);
                    connections[connectionString] = connection;
                }
            }

            return connection;
        }

        private void DisposeConnection(TDbConnection connection) {
            if(!reuseConnection) {
                connection.Dispose();
            }
        }
    }
}
