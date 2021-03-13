using System;
using System.Collections.Generic;

namespace integrateDb.Models {
    public class Column {
        public string Name { get; init; }
        public string Value { get; init; }
        public string? ConverterType { get; set; }

        public Column(string name, string value) {
            Name = name;
            Value = value;
        }
    }
}
