using System;

namespace integrateDb.Formatter {
    public class DateTimeFormatter {
        private readonly string dateWithTimeFormat;
        private readonly string dateWithoutTimeFormat;

        public DateTimeFormatter(string dateWithTimeFormat) {
            this.dateWithTimeFormat = dateWithTimeFormat;
        }

        public DateTimeFormatter(string dateWithTimeFormat, string dateWithoutTimeFormat) {
            this.dateWithTimeFormat = dateWithTimeFormat;
            this.dateWithoutTimeFormat = dateWithoutTimeFormat;
        }

        public string Format(object value) {
            var date = (DateTime)value;
            if(date.TimeOfDay != TimeSpan.Zero)
                return date.ToString(dateWithTimeFormat);

            if(string.IsNullOrWhiteSpace(dateWithoutTimeFormat))
                return date.ToString(dateWithTimeFormat);
            else
                return date.ToString(dateWithoutTimeFormat);
        }
    }
}
