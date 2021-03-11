using System;
namespace integrateDb.Formatter {
    public class DecimalFormatter {
        private readonly string format;

        public DecimalFormatter(string format) {
            this.format = format;
        }

        public string Format(object value) {
            return ((decimal)value).ToString(format);
        }
    }
}
