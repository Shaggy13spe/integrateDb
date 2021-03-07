using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using integrateDb.DatabaseClient;
using integrateDb.Managers;

using Microsoft.Data.SqlClient;

namespace integrateDb.SqlServer {
    public class SqlDatabaseClient : AdoDatabaseClient<SqlConnection, SqlCommand> {

        public SqlDatabaseClient(bool reuseConnection) : base(reuseConnection) { }

        public override JsonElement ReadTableData(string command, FormatterManager formatter) {
            using var memoryStream = new MemoryStream();
            using var writer = new Utf8JsonWriter(memoryStream, new JsonWriterOptions { Indented = true });

            using var connection = CreateDbConnection(ConnectionString);
            connection.Open();
            using var sqlCommand = CreateDbCommand(command, connection);
            using var reader = sqlCommand.ExecuteReader(CommandBehavior.KeyInfo);
            var tableSchema = reader.GetSchemaTable();

            writer.WriteStartObject();
            writer.WritePropertyName("table");
            writer.WriteStringValue(tableSchema.Rows[0]["BaseTableName"].ToString());

            writer.WriteStartArray("rows");
            while(reader.Read()) {
                writer.WriteStartObject();
                writer.WriteStartArray("columns");

                for(var i = 0; i < reader.VisibleFieldCount; i++) {
                    var columnName = reader.GetName(i);
                    var value = formatter.Format(tableSchema.TableName, columnName, reader[i]);

                    writer.WriteStartObject();
                    writer.WritePropertyName("name");
                    writer.WriteStringValue(columnName);
                    writer.WritePropertyName("value");
                    writer.WriteStringValue(value);
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();

            writer.Flush();

            var jsonString = Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);

            var document = JsonDocument.Parse(jsonString);
            return document.RootElement;

        }

        public override void InsertTableData(JsonElement dataset) {

            var stringBuilder = new StringBuilder();
            var tableName = dataset.GetProperty("table").GetString();

            var setIdentityInsertExists = dataset.TryGetProperty("setIdentityInsert", out JsonElement setIdentityInsert);
            if(setIdentityInsertExists && setIdentityInsert.GetBoolean()) {
                var setIdentitySql = $"SET IDENTITY_INSERT {tableName} ON;";
                stringBuilder.AppendLine(setIdentitySql);
                stringBuilder.AppendLine();
            }

            foreach(var row in dataset.GetProperty("rows").EnumerateArray()) {
                var queryFieldNames = string.Join(", ",
                    row.GetProperty("columns").EnumerateArray().Select(a =>
                        a.GetProperty("name").GetString()).ToArray());

                var queryFieldValues = string.Join(", ",
                    row.GetProperty("columns").EnumerateArray().Select(a => {
                        var value = a.GetProperty("value").GetString();
                        var converterTypeExists = a.TryGetProperty("converterType", out JsonElement converterType);
                        if(converterTypeExists)
                            return $"CONVERT({converterType.GetString()}, '{value}', 1)";
                        else
                            return $"'{value}'";
                    }).ToArray());

                var insertSql = $"INSERT INTO {tableName} ({queryFieldNames}) VALUES ({queryFieldValues});";
                stringBuilder.AppendLine(insertSql);
            }

            if(setIdentityInsertExists && setIdentityInsert.GetBoolean()) {
                var setIdentitySql = $"SET IDENTITY_INSERT {tableName} OFF;";
                stringBuilder.AppendLine(setIdentitySql);
                stringBuilder.AppendLine();
            }

            ExecuteCommand(stringBuilder.ToString());

        }

        protected override SqlConnection CreateDbConnection(string connectionString) => new SqlConnection(connectionString);
        protected override SqlCommand CreateDbCommand(string command, SqlConnection connection) => new SqlCommand(command, connection);
    }
}
