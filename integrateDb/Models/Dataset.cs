using System;
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
            var result = new Dataset();
            var datasetName = datasetJson.GetProperty("name").GetString();

            if(!datasetJson.TryGetProperty("table", out _))
                throw new InvalidOperationException($"Dataset '{datasetName}' is missing a table property");

            if(!datasetJson.TryGetProperty("rows", out _))
                throw new InvalidOperationException($"Dataset '{datasetName}' is missing a rows array");

            result.Rows = new();
            var rowCount = 0;
            foreach(var row in datasetJson.GetProperty("rows").EnumerateArray()) {
                if(!row.TryGetProperty("columns", out _))
                    throw new InvalidOperationException($"Dataset '{datasetName}', [row {rowCount}] is missing a columns array");

                var columns = new List<Column>();
                var colCount = 0;
                foreach(var column in row.GetProperty("columns").EnumerateArray()) {
                    var nameExists = column.TryGetProperty("name", out var name);
                    var valueExists = column.TryGetProperty("value", out var value);
                    if(nameExists && valueExists) {
                        if(string.IsNullOrWhiteSpace(name.GetString()))
                            throw new InvalidOperationException($"Dataset 'test-dataset', [row {rowCount}, column {colCount}], name cannot be null or empty string");


                        columns.Add(new(name.GetString()!, value.GetString()!));
                        colCount++;
                    }
                    if(name.ValueKind == JsonValueKind.Undefined)
                        throw new InvalidOperationException($"Dataset 'test-dataset', [row {rowCount}, column {colCount}] is missing a name property");

                    if(value.ValueKind == JsonValueKind.Undefined)
                        throw new InvalidOperationException($"Dataset 'test-dataset', [row {rowCount}, column {colCount}] is missing a value property");

                }
                result.Rows.Add(new Row { Columns = columns });
                rowCount++;
            }

            return result;
        }
    }
}
