using System.Text.Json;

using FluentAssertions;

using integrateDb.Formatter;

using Xunit;

namespace integrateDbTests.Formatters {
    public class ActionFormatterTest {

        [Fact]
        public void Format_WhenCalledWithGivenFunc_ReturnsCorrectFormat() {
            static string action(object value) {
                if(value == null)
                    return "Cannot format null value";

                if(value.GetType() == typeof(byte[])) {
                    var jsonString = JsonSerializer.Serialize(value);
                    return jsonString.Replace("\"", "");
                }
                else return value.ToString();
            }

            var formatter = new ActionFormatter(action);

            var actual = formatter.Format(new byte[] { 0x0FD });
            actual.Should().Be("/Q==");

            actual = formatter.Format(new object());
            actual.Should().Be("System.Object");

            actual = formatter.Format(null);
            actual.Should().Be("Cannot format null value");
        }
    }
}
