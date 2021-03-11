using System;

using FluentAssertions;

using integrateDb.Formatter;

using Xunit;

namespace integrateDbTests.Formatters {
    public class DecimalFormatterTest {

        [Fact]
        public void Format_WhenCalled_ReturnsCorrectFormat() {
            var value = 10.1234m;
            var formatter = new DecimalFormatter("0.00");

            var actual = formatter.Format(value);
            actual.Should().Be("10.12");
        }
    }
}
