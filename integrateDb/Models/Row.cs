using System;
using System.Collections.Generic;
using System.Text.Json;

namespace integrateDb.Models {
    public class Row {
        public List<Column> Columns { get; set; } = new();

        public Column? FindColumn(string columnName) {
            return Columns.Find(x => x.Name == columnName);
        }
    }
}
