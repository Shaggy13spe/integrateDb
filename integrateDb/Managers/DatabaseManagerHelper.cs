using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using FluentAssertions;
using FluentAssertions.Execution;

namespace integrateDb.Managers {
    public static class DatabaseManagerHelper {
        public static void CompareDatasets(JsonElement expected, JsonElement actual, string[] keys, bool sorted, bool compareTableName = true) {
            var expectedTableName = expected.GetProperty("table").GetString();
            var actualTableName = actual.GetProperty("table").GetString();
            var expectedRowCount = expected.GetProperty("rows").GetArrayLength();
            var actualRowCount = actual.GetProperty("rows").GetArrayLength();
            var expectedRows = expected.GetProperty("rows");
            var actualRows = actual.GetProperty("rows");

            if(compareTableName)
                expectedTableName.Should().Be(actualTableName);

            expectedRowCount.Should().Be(actualRowCount);

            if(sorted)
                CompareSortedElements(expectedRows, actualRows);
            else
                CompareUnsortedElements(expectedRows, actualRows, keys);
        }

        private static void CompareUnsortedElements(JsonElement expectedRows, JsonElement actualRows, string[] keys) {
            foreach(var expectedRow in expectedRows.EnumerateArray()) {
                var expectedKeyProperties = new List<JsonElement>();
                foreach(var key in keys) {
                    var columns = expectedRow.GetProperty("columns");
                    foreach(var column in columns.EnumerateArray()) {
                        if(column.GetProperty("name").GetString() == key) {
                            expectedKeyProperties.Add(column);
                        }
                    }
                }

                var actualRow = FindActualRow(expectedKeyProperties, actualRows);
                CompareElements(expectedRow, actualRow.Value);
            }
        }

        private static JsonElement? FindActualRow(List<JsonElement> expectedKeyProperties, JsonElement actualRows) {
            JsonElement? result = null;
            foreach(var actualRow in actualRows.EnumerateArray()) {
                var actualKeyExists = false;
                foreach(var expectedKeyProperty in expectedKeyProperties) {
                    var columns = actualRow.GetProperty("columns");
                    foreach(var column in columns.EnumerateArray()) {
                        if(column.GetProperty("name").GetString() == expectedKeyProperty.GetProperty("name").GetString()) {
                            actualKeyExists = true;
                            break;
                        }
                    }
                    actualKeyExists.Should().BeTrue();

                    var actualKeyProperties = new List<JsonElement>();
                    foreach(var column in columns.EnumerateArray()) {
                        if(column.GetProperty("name").GetString() == expectedKeyProperty.GetProperty("name").GetString() && column.GetProperty("value").ToString() == expectedKeyProperty.GetProperty("value").ToString())
                            actualKeyProperties.Add(column);
                    }
                    actualKeyProperties.Count.Should().NotBe(0, "because then the row wasn't found");
                    actualKeyProperties.Count.Should().Be(1, "because we can't have more than one row with the same key values");

                    result = actualRow;
                }
                if(result.HasValue)
                    break;
            }
            return result;
        }

        private static void CompareSortedElements(JsonElement expectedRows, JsonElement actualRows) {
            using var expectedEnumerator = expectedRows.EnumerateArray();
            using var actualEnumerator = actualRows.EnumerateArray();
            for(var i = 0; i < expectedRows.GetArrayLength(); i++) {
                var expectedRow = expectedEnumerator.ElementAt(i);
                var actualRow = actualEnumerator.ElementAt(i);
                CompareElements(expectedRow, actualRow);
            }
        }

        private static void CompareElements(JsonElement expectedRow, JsonElement actualRow) {

            using var assertionScope = new AssertionScope();
            using var expectedEnumerator = expectedRow.EnumerateObject();
            using var actualEnumerator = actualRow.EnumerateObject();
            var expectedColumnCount = expectedEnumerator.Count();
            var actualColumnCount = actualEnumerator.Count();

            expectedColumnCount.Should().Be(actualColumnCount);

            foreach(var expectedProperty in expectedEnumerator) {
                var actualProperty = actualEnumerator.FirstOrDefault(a => a.Name == expectedProperty.Name);
                actualProperty.Should().NotBeNull("because you expected it to be in the actual resultset");
                expectedProperty.Value.Should().Be(actualProperty.Value);
            }

        }
    }
}
