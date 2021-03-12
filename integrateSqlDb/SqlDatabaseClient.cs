using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using integrateDb.DatabaseClient;
using integrateDb.Managers;
using integrateDb.Models;

using Microsoft.Data.SqlClient;

namespace integrateDb.SqlServer {
    public class SqlDatabaseClient : AdoDatabaseClient<SqlConnection, SqlCommand> {

        public SqlDatabaseClient(bool reuseConnection) : base(reuseConnection) { }

        public override Dataset ReadTableData(string command, FormatterManager formatter) {

            using var connection = CreateDbConnection(ConnectionString);
            connection.Open();
            using var sqlCommand = CreateDbCommand(command, connection);
            using var reader = sqlCommand.ExecuteReader(CommandBehavior.KeyInfo);
            var tableName = reader.GetSchemaTable().Rows[0]["BaseTableName"].ToString();

            var dataset = new Dataset();
            var rows = new List<Row>();
            while(reader.Read()) {
                var columns = new List<Column>();

                for(var i = 0; i < reader.VisibleFieldCount; i++) {
                    var column = new Column(reader.GetName(i), formatter.Format(tableName ?? "", reader.GetName(i), reader[i]));
                    columns.Add(column);
                }

                var row = new Row { Columns = columns };
                rows.Add(row);
            }

            dataset.Rows = rows;
            return dataset;
        }

        public override void InsertTableData(Dataset dataset) {

            var stringBuilder = new StringBuilder();
            var tableName = dataset.Table;

            if(dataset.SetIdentityInsert) {
                var setIdentitySql = $"SET IDENTITY_INSERT {tableName} ON;";
                stringBuilder.AppendLine(setIdentitySql);
                stringBuilder.AppendLine();
            }

            foreach(var insertSql in from row in dataset.Rows
                                     let queryFieldNames = string.Join(", ",
                                        row.Columns.Select(a =>
                                            a.Name).ToArray())
                                     let queryFieldValues = string.Join(", ",
                                        row.Columns.Select(a => {
                                            var value = a.Value;
                                            if(a.ConverterType != null)
                                                return $"CONVERT({a.ConverterType}, '{value}', 1)";
                                            else
                                                return $"'{value}'";
                                        }).ToArray())
                                     let insertSql = $"INSERT INTO {tableName} ({queryFieldNames}) VALUES ({queryFieldValues});"
                                     select insertSql) {
                stringBuilder.AppendLine(insertSql);
            }

            if(dataset.SetIdentityInsert) {
                var setIdentitySql = $"SET IDENTITY_INSERT {tableName} OFF;";
                stringBuilder.AppendLine(setIdentitySql);
                stringBuilder.AppendLine();
            }

            ExecuteCommand(stringBuilder.ToString());

        }

        protected override SqlConnection CreateDbConnection(string connectionString) => new(connectionString);
        protected override SqlCommand CreateDbCommand(string command, SqlConnection connection) => new(command, connection);
    }
}
