using System;
using System.Text.Json;

using FluentAssertions;

using integrateDb.Managers;
using integrateDb.Models;
using integrateDb.SqlServer;

using Xunit;

namespace integrateSqlDbTests {
    public class SqlDatabaseClientTest {
        private readonly string connectionString = "Server=localhost, 1433;Database=integrateDbTest;User Id=sa;Password=#SAPassword!;";
        private readonly string createGlobalTempTableCommand = @"
                IF OBJECT_ID('tempdb.dbo.##GlobalTempTable') IS NOT NULL
                BEGIN
                    DROP TABLE ##GlobalTempTable
                END;

                CREATE TABLE ##GlobalTempTable (col1 INT PRIMARY KEY);";
        private readonly string verifyIfTempTableExistsQuery = @"
                IF OBJECT_ID('tempdb.dbo.##GlobalTempTable') IS NOT NULL
                    SELECT '1' AS TempTableExists
                ELSE
                    SELECT '0' AS TempTableExists;";




        [Fact]
        public void Connection_Is_Not_Reused() {
            var client = new SqlDatabaseClient(false) { ConnectionString = connectionString };
            client.ExecuteCommand(createGlobalTempTableCommand);

            var formatterManager = new FormatterManager();
            var actualDataset = client.ReadTableData(verifyIfTempTableExistsQuery, formatterManager);

            var expectedDatasetJson = @"
                 {
                    ""name"": ""globalTempTable"",
                    ""table"": ""GlobalTempTable"",
                    ""rows"": [
                        {
                        ""columns"": [
                                {
                                    ""name"": ""TempTableExists"",
                                    ""value"": ""0""
                                }
                            ]
                        }
                    ]
                }";
            var jsonDoc = JsonDocument.Parse(expectedDatasetJson);
            var expectedDataset = Dataset.Parse(jsonDoc.RootElement);

            expectedDataset.Should().BeEquivalentTo(actualDataset);
        }

        [Fact]
        public void Connection_Is_Reused_When_Configured() {
            var client = new SqlDatabaseClient(true) { ConnectionString = connectionString };
            client.ExecuteCommand(createGlobalTempTableCommand);

            var formatterManager = new FormatterManager();
            var actualDataset = client.ReadTableData(verifyIfTempTableExistsQuery, formatterManager);

            var expectedDatasetJson = @"
                 {
                    ""name"": ""globalTempTable"",
                    ""table"": ""GlobalTempTable"",
                    ""rows"": [
                        {
                        ""columns"": [
                                {
                                    ""name"": ""TempTableExists"",
                                    ""value"": ""1""
                                }
                            ]
                        }
                    ]
                }";
            var jsonDoc = JsonDocument.Parse(expectedDatasetJson);
            var expectedDataset = Dataset.Parse(jsonDoc.RootElement);

            expectedDataset.Should().BeEquivalentTo(actualDataset);
        }
    }
}
