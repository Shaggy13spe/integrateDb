using System;
using System.Text.Json;

using FluentAssertions;

using integrateDb.Managers;
using integrateDb.Models;
using integrateDb.SqlServer;

using Microsoft.Data.SqlClient;

using Xunit;

namespace integrateSqlDbTests {
    public class SqlDatabaseClientTest {
        private readonly string connectionString = "Server=localhost;Database=integrateDbTest;User Id=sa;Password=#SAPassword!;";
        private readonly string createTempTableCommand = "CREATE TABLE #TempTable (col1 INT);";
        private readonly string verifyIfTempTableExistsQuery = "INSERT INTO #TempTable (col1) VALUES (1);";

        [Fact]
        public void Connection_Is_Not_Reused() {
            var client = new SqlDatabaseClient(false) { ConnectionString = connectionString };
            client.ExecuteCommand(createTempTableCommand);

            Action action = () => client.ExecuteCommand(verifyIfTempTableExistsQuery);
            action.Should().Throw<SqlException>();
        }

        [Fact]
        public void Connection_Is_Reused_When_Configured() {
            var client = new SqlDatabaseClient(true) { ConnectionString = connectionString };
            client.ExecuteCommand(createTempTableCommand);

            Action action = () => client.ExecuteCommand(verifyIfTempTableExistsQuery);
            action.Should().NotThrow();
        }

        [Fact(Skip = "need to setup sql database")]
        public void Write_Read_And_Compare_Results() {
            var client = new SqlDatabaseClient(false) { ConnectionString = connectionString };
            var deleteCmd = @"
                DELETE FROM public.product;
                DELETE FROM public.category;
                DELETE FROM public.supplier;
            ";

            client.ExecuteCommand(deleteCmd);

            var expectedDatasetJson = @"
                 {
                    ""name"": ""suppliers"",
                    ""table"": ""Supplier"",
                    ""rows"": [
                        {
                        ""columns"": [
                                {
                                    ""name"": ""id"",
                                    ""value"": ""1""
                                },
                                {
                                    ""name"": ""name"",
                                    ""value"": ""supplier-1""
                                },
                                {
                                    ""name"": ""contact_name"",
                                    ""value"": ""contact-name-1""
                                },
                                {
                                    ""name"": ""contact_phone"",
                                    ""value"": ""100-200-0001""
                                },
                                {
                                    ""name"": ""contact_email"",
                                    ""value"": ""email-1@test.com""
                                }
                            ]
                        },
                        {
                        ""columns"": [
                                {
                                    ""name"": ""id"",
                                    ""value"": ""2""
                                },
                                {
                                    ""name"": ""name"",
                                    ""value"": ""supplier-2""
                                },
                                {
                                    ""name"": ""contact_name"",
                                    ""value"": ""contact-name-2""
                                },
                                {
                                    ""name"": ""contact_phone"",
                                    ""value"": ""100-200-0002""
                                },
                                {
                                    ""name"": ""contact_email"",
                                    ""value"": ""email-2@test.com""
                                }
                            ]
                        },
                        {
                        ""columns"": [
                                {
                                    ""name"": ""id"",
                                    ""value"": ""3""
                                },
                                {
                                    ""name"": ""name"",
                                    ""value"": ""supplier-3""
                                },
                                {
                                    ""name"": ""contact_name"",
                                    ""value"": ""contact-name-3""
                                },
                                {
                                    ""name"": ""contact_phone"",
                                    ""value"": ""100-200-0003""
                                },
                                {
                                    ""name"": ""contact_email"",
                                    ""value"": ""email-3@test.com""
                                }
                            ]
                        }
                    ]
                }
            ";

            var jsonDoc = JsonDocument.Parse(expectedDatasetJson);
            var expectedDataset = Dataset.Parse(jsonDoc.RootElement);

            client.InsertTableData(expectedDataset);

            var formatterManager = new FormatterManager();
            var selectQuery = "SELECT * FROM public.supplier;";

            var actualDataset = client.ReadTableData(selectQuery, formatterManager);

            expectedDataset.Rows.Should().BeEquivalentTo(actualDataset.Rows);

        }
    }
}
