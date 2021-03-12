using System.Text.Json;

namespace integrateDb.Models {
    public class Script {
        public string Name { get; private set; } = "";
        public string Value { get; private set; } = "";

        public static Script Parse(JsonElement scriptJson) {
            //Validation code here

            var result = new Script();
            if(scriptJson.TryGetProperty("name", out var name) && scriptJson.TryGetProperty("value", out var value)) {
                result.Name = name.GetString()!;
                result.Value = value.GetString()!;
            }

            return result;
        }
    }
}
