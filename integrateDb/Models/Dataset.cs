using System.Collections.Generic;
using System.Text.Json;

namespace integrateDb.Models {
    public class Dataset {

        public string? Name { get; set; }
        public string? Table { get; set; }
        public string Schema { get; set; } = "dbo";
        public bool SetIdentityInsert { get; set; }
        public List<Row> Rows { get; set; } = new();

        public static Dataset Parse(JsonElement datasetJson) {
            //Validation code here

            var result = new Dataset();
            result.Rows = new();
            foreach(var row in datasetJson.GetProperty("rows").EnumerateArray()) {
                var columns = new List<Column>();
                foreach(var column in row.GetProperty("columns").EnumerateArray()) {
                    if(column.TryGetProperty("name", out var name) && column.TryGetProperty("value", out var value)) {
                        columns.Add(new(name.GetString()!, value.GetString()!));
                    }
                }
                result.Rows.Add(new Row { Columns = columns });
            }
            return result;
        }
    }
}
