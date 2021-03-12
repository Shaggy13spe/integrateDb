using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using FluentAssertions.Execution;

using integrateDb.Models;

namespace integrateDb.Managers {
    public static class DatabaseManagerHelper {
        public static void CompareDatasets(Dataset expected, Dataset actual, string[] keys, bool sorted, bool compareTableName = true) {
            var expectedTableName = expected.Table;
            var actualTableName = actual.Table;

            if(compareTableName)
                expectedTableName.Should().Be(actualTableName, "because table names should be the same");

            if(expected.Rows.Count == 0) {
                actual.Rows.Count.Should().Be(0, "because expected data is empty");
            }
            else
                actual.Rows.Count.Should().BeGreaterThan(0, "because expected data is not empty");

            expected.Rows.Count.Should().Be(actual.Rows.Count);

            if(sorted)
                CompareSortedElements(expected.Rows, actual.Rows);
            else
                CompareUnsortedElements(expected.Rows, actual.Rows, keys);
        }

        private static void CompareUnsortedElements(List<Row> expectedRows, List<Row> actualRows, string[] keys) {
            foreach(var (expectedRow, expectedKeyColumns) in from expectedRow in expectedRows
                                                             let expectedKeyColumns = new List<Column>()
                                                             select (expectedRow, expectedKeyColumns)) {
                expectedKeyColumns.AddRange(from key in keys
                                            let columns = expectedRow.Columns
                                            from column in columns
                                            where column.Name == key
                                            select column);
                var actualRow = FindActualRowByKeys(expectedKeyColumns, actualRows);
                CompareElements(expectedRow, actualRow!);
            }
        }

        private static Row? FindActualRowByKeys(List<Column> expectedKeyColumns, List<Row> actualRows) {
            Row? result = null;
            foreach(var row in actualRows) {
                foreach(var key in expectedKeyColumns) {
                    var keyColumn = row.Columns.Find(a => a.Name == key.Name);
                    keyColumn.Should().NotBeNull($"because actual data is missing key: {key.Name}");
                }
            }

            var possibleKeyColumns = new List<Column>();
            foreach(var row in actualRows!) {
                foreach(var key in expectedKeyColumns) {
                    var possibleKeyColumn = row.Columns.Find(a => a.Name == key.Name && a.Value == key.Value);
                    if(possibleKeyColumn != null) {
                        possibleKeyColumns.Add(possibleKeyColumn);
                    }
                }
                if(possibleKeyColumns.Count == expectedKeyColumns.Count) {
                    result = row;
                    break;
                }
            }

            result.Should().NotBeNull();

            return result;
        }

        private static void CompareSortedElements(List<Row> expectedRows, List<Row> actualRows) {
            for(var i = 0; i < expectedRows.Count; i++) {
                var expectedRow = expectedRows.ElementAt(i);
                var actualRow = actualRows.ElementAt(i);
                CompareElements(expectedRow, actualRow!);
            }
        }

        private static void CompareElements(Row expectedRow, Row actualRow) {

            using var assertionScope = new AssertionScope();
            expectedRow.Columns.Count.Should().Be(actualRow.Columns.Count);

            if(expectedRow.Columns != null)
                foreach(var expectedColumn in expectedRow.Columns) {
                    if(expectedColumn.Name != null) {
                        var actualColumn = actualRow.FindColumn(expectedColumn.Name);
                        actualColumn.Should().NotBeNull($"because column: {expectedColumn} isn't found in actual dataset");
                        expectedColumn.Should().BeEquivalentTo(actualColumn);
                    }
                }
        }
    }
}
