using System;
using System.Text.Json;

using FluentAssertions;

using integrateDb.Models;

using Xunit;

namespace integrateDbTests.Models {
    public class ScriptTest {
        [Fact]
        public void Parse_WhenCalledWithoutValue_ThenFail() {
            var scriptJson = @"
            {
                ""name"": ""test-script""
            }";

            var jsonDoc = JsonDocument.Parse(scriptJson);
            var root = jsonDoc.RootElement;

            Action action = () => Script.Parse(root);

            action.Should().Throw<InvalidOperationException>().WithMessage("Script 'test-script' is missing a value property");
        }

        [Fact]
        public void Parse_WhenCalledWithEmptyValue_ThenFail() {
            var scriptJson = @"
            {
                ""name"": ""test-script"",
                ""value"": """"
            }";

            var jsonDoc = JsonDocument.Parse(scriptJson);
            var root = jsonDoc.RootElement;

            Action action = () => Script.Parse(root);

            action.Should().Throw<InvalidOperationException>().WithMessage("Script 'test-script', value cannot be null or empty string");
        }

        [Fact]
        public void Parse_WhenCalledWithValue_ThenSucceed() {
            var scriptJson = @"
            {
                ""name"": ""test-script"",
                ""value"": ""some db command""
            }";

            var jsonDoc = JsonDocument.Parse(scriptJson);
            var root = jsonDoc.RootElement;

            Action action = () => Script.Parse(root);

            action.Should().NotThrow();
        }
    }
}
