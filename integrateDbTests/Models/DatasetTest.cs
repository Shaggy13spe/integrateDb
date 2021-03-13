using System;
using System.Text.Json;

using FluentAssertions;

using integrateDb.Models;

using Xunit;

namespace integrateDbTests.Models {
    public class DatasetTest {
        [Fact]
        public void Parse_WhenCalledWithoutTable_ThenFail() {
            var datasetJson = @"
            {
                ""name"": ""test-dataset""
            }";

            var jsonDoc = JsonDocument.Parse(datasetJson);
            var root = jsonDoc.RootElement;

            Action action = () => Dataset.Parse(root);

            action.Should().Throw<InvalidOperationException>().WithMessage("Dataset 'test-dataset' is missing a table property");
        }

        [Fact]
        public void Parse_WhenCalledWithoutRows_ThenFail() {
            var datasetJson = @"
            {
                ""name"": ""test-dataset"",
                ""table"": ""testTable""
            }";

            var jsonDoc = JsonDocument.Parse(datasetJson);
            var root = jsonDoc.RootElement;

            Action action = () => Dataset.Parse(root);

            action.Should().Throw<InvalidOperationException>().WithMessage("Dataset 'test-dataset' is missing a rows array");
        }

        [Fact]
        public void Parse_WhenCalledWithoutColumns_ThenFail() {
            var datasetJson = @"
            {
                ""name"": ""test-dataset"",
                ""table"": ""testTable"",
                ""rows"": [
                    {
                        ""columns"" : []
                    },
                    {
                    }
                ]
            }";

            var jsonDoc = JsonDocument.Parse(datasetJson);
            var root = jsonDoc.RootElement;

            Action action = () => Dataset.Parse(root);

            action.Should().Throw<InvalidOperationException>().WithMessage("Dataset 'test-dataset', [row 1] is missing a columns array");
        }

        [Fact]
        public void Parse_WhenCalledWithoutColumnName_ThenFail() {
            var datasetJson = @"
            {
                ""name"": ""test-dataset"",
                ""table"": ""testTable"",
                ""rows"": [
                    {
                        ""columns"" : [
                            {}
                        ]
                    }
                ]
            }";

            var jsonDoc = JsonDocument.Parse(datasetJson);
            var root = jsonDoc.RootElement;

            Action action = () => Dataset.Parse(root);

            action.Should().Throw<InvalidOperationException>().WithMessage("Dataset 'test-dataset', [row 0, column 0] is missing a name property");
        }

        [Fact]
        public void Parse_WhenCalledWithoutColumnValue_ThenFail() {
            var datasetJson = @"
            {
                ""name"": ""test-dataset"",
                ""table"": ""testTable"",
                ""rows"": [
                    {
                        ""columns"" : [
                            {
                                ""name"": ""Column1""
                            }
                        ]
                    }
                ]
            }";

            var jsonDoc = JsonDocument.Parse(datasetJson);
            var root = jsonDoc.RootElement;

            Action action = () => Dataset.Parse(root);

            action.Should().Throw<InvalidOperationException>().WithMessage("Dataset 'test-dataset', [row 0, column 0] is missing a value property");
        }

        [Fact]
        public void Parse_WhenCalledWithEmptyColumnName_ThenFail() {
            var datasetJson = @"
            {
                ""name"": ""test-dataset"",
                ""table"": ""testTable"",
                ""rows"": [
                    {
                        ""columns"" : [
                            {
                                ""name"": """",
                                ""value"": ""a value""
                            }
                        ]
                    }
                ]
            }";

            var jsonDoc = JsonDocument.Parse(datasetJson);
            var root = jsonDoc.RootElement;

            Action action = () => Dataset.Parse(root);

            action.Should().Throw<InvalidOperationException>().WithMessage("Dataset 'test-dataset', [row 0, column 0], name cannot be null or empty string");
        }

        [Fact]
        public void Parse_WhenCalledWithRequiredProperties_ThenSucceed() {
            var datasetJson = @"
            {
                ""name"": ""test-dataset"",
                ""table"": ""testTable"",
                ""rows"": [
                    {
                        ""columns"" : [
                            {
                                ""name"": ""Column1"",
                                ""value"": ""a value""
                            }
                        ]
                    }
                ]
            }";

            var jsonDoc = JsonDocument.Parse(datasetJson);
            var root = jsonDoc.RootElement;

            Action action = () => Dataset.Parse(root);

            action.Should().NotThrow();
        }
    }
}
