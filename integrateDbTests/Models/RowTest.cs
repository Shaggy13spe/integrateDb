using System.Collections.Generic;

using FluentAssertions;

using integrateDb.Models;

using Xunit;

namespace integrateDbTests.Models {
    public class RowTest {
        [Fact]
        public void FindColumn_WhenColumnExists_ReturnsColumn() {
            var columns = new List<Column> {
                new("Column1", "1"),
                new("Column2", "2"),
                new("Column3", "3")
            };
            var row = new Row { Columns = columns };
            var expected = row.Columns[1];

            var actual = row.FindColumn("Column2");

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void FindColumn_WhenColumnNotExists_ReturnsNull() {
            var columns = new List<Column> {
                new("Column1", "1"),
                new("Column2", "2"),
                new("Column3", "3")
            };
            var row = new Row { Columns = columns };

            var actual = row.FindColumn("Column4");

            actual.Should().BeNull();
        }
    }
}
