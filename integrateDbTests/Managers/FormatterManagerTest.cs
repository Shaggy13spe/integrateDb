using System;

using FluentAssertions;

using integrateDb.Interfaces;
using integrateDb.Managers;

using Xunit;

namespace integrateDbTests.Managers {
    public class FormatterManagerTest {
        private FormatterManager formatterManager;

        public FormatterManagerTest() {
            formatterManager = new FormatterManager();

            formatterManager.Register(typeof(DateTime), value => "registered-for-date-time");
            formatterManager.Register("TableA", "ColumnA", value => "registered-for-TableA-ColumnA");
            formatterManager.Register("TableA", "ColumnB", value => "registered-for-TableA-ColumnB");
            formatterManager.Register("TableB", "ColumnA", value => "registered-for-TableB-ColumnA");
            formatterManager.Register("ColumnA", value => "registered-for-ColumnA");
        }

        [Fact]
        public void Format_WhenNoFormattersAreRegistered_ReturnValueAsString() {
            formatterManager = new FormatterManager();

            var actual = formatterManager.Format("TableB", "ColumnA", 5);
            actual.Should().Be("5", "because there aren't any formatters registered");
        }

        [Fact]
        public void Format_WhenRegisteredFormatterIsNotFound_ReturnValueAsString() {
            var actual = formatterManager.Format("TableB", "ColumnB", 5);
            actual.Should().Be("5", "because a registered formatter could not be found that matches");
        }

        [Fact]
        public void Format_WhenRegisteredFormatterForTableAndColumnIsFound_ItUsesTheFormatter() {
            var actual = formatterManager.Format("TableA", "ColumnB", 5);
            actual.Should().Be("registered-for-TableA-ColumnB");
        }

        [Fact]
        public void Format_WhenRegisteredFormatterForColumnIsFound_ItUsesTheFormatter() {
            var actual = formatterManager.Format("TableC", "ColumnA", 5);
            actual.Should().Be("registered-for-ColumnA");
        }

        [Fact]
        public void Format_WhenRegisteredFormatterForTypeIsFound_ItUsesTheFormatter() {
            var actual = formatterManager.Format("TableC", "ColumnC", DateTime.Now);
            actual.Should().Be("registered-for-date-time");
        }

        [Fact]
        public void Format_WhenMoreThanOneFormatterIsFound_ItUsesTheFormatter_ByOrderOfPrecedence() {
            var actual = formatterManager.Format("TableA", "ColumnA", DateTime.Now);
            actual.Should().Be("registered-for-TableA-ColumnA", "because table & column takes precedence over column & type formatters");

            actual = formatterManager.Format("TableC", "ColumnA", DateTime.Now);
            actual.Should().Be("registered-for-ColumnA", "because column takes precedence over type formatters");
        }

        [Fact]
        public void Format_WhenRegisteredFormatterInterfaceIsFound_ItUsesTheFormatter() {
            var formatter = new FakeIntFormatter();
            formatterManager.Register(typeof(int), formatter);
            var value = 10;
            var actual = formatterManager.Format("TableC", "ColumnC", value);

            actual.Should().Be("int-formatter");
            formatter.LastValue.Should().Be(10);
        }

        private class FakeIntFormatter : IColumnFormatter {
            public object LastValue { get; set; }

            public string Format(object value) {
                LastValue = value;
                return "int-formatter";
            }
        }

        [Fact]
        public void Format_WhenRegisteredFormatterFuncIsFound_ItUsesTheFormatter() {
            formatterManager.Register(typeof(double), (value) => {
                if(value.GetType() == typeof(double))
                    return "double-formatter";
                else
                    return "value-not-a-double";
            });

            var value = 10.0;
            var actual = formatterManager.Format("TableC", "ColumnC", value);
            actual.Should().Be("double-formatter");
        }
    }
}
