using System;

using FluentAssertions;

using integrateDb.Formatter;

using Xunit;

namespace integrateDbTests.Formatters {
    public class DateTimeFormatterTest {
        private DateTime dateTime = new(2000, 1, 2, 3, 4, 5);
        private DateTime date = new(2001, 1, 2);

        [Fact]
        public void Format_WhenCalledWithOneFormat_ReturnsCorrectFormat() {
            var formatter = new DateTimeFormatter("yyyy-MM-dd HH:mm");

            var actual = formatter.Format(date);
            actual.Should().Be("2001-01-02 00:00");

            actual = formatter.Format(dateTime);
            actual.Should().Be("2000-01-02 03:04");
        }

        [Fact]
        public void Format_WhenCalledWithTwoFormats_ReturnsCorrectFormats() {
            var formatter = new DateTimeFormatter("yyyy-MM-dd HH:mm", "yyyy-MM-dd");

            var actual = formatter.Format(date);
            actual.Should().Be("2001-01-02");

            actual = formatter.Format(dateTime);
            actual.Should().Be("2000-01-02 03:04", actual);
        }
    }
}
