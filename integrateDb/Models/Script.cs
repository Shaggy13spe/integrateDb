using System;
using System.Text.Json;

namespace integrateDb.Models {
    public class Script {
        public string Name { get; private set; } = "";
        public string Value { get; private set; } = "";

        public static Script Parse(JsonElement scriptJson) {
            var result = new Script();
            var name = scriptJson.GetProperty("name").GetString();
            if(scriptJson.TryGetProperty("value", out var value)) {
                if(string.IsNullOrWhiteSpace(value.GetString()))
                    throw new InvalidOperationException($"Script '{name}', value cannot be null or empty string");

                result.Name = name!;
                result.Value = value.GetString()!;
                return result;
            }

            throw new InvalidOperationException($"Script '{name}' is missing a value property");
        }
    }
}
