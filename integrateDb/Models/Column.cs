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

    public class ColumnKeyComparer : IEqualityComparer<Column> {
        public int GetHashCode(Column col) {
            if(col == null)
                return 0;

            return col.Name.GetHashCode();
        }

        public bool Equals(Column? col1, Column? col2) {
            if(ReferenceEquals(col1, col2))
                return true;

            if(col1 is null || col2 is null)
                return true;

            return col1.Name == col2.Name;
        }
    }
}
