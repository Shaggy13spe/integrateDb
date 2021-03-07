using System;

using integrateDb.Interfaces;

namespace integrateDb.Formatter {
    public class ActionFormatter : IColumnFormatter {
        private readonly Func<object, string> func;

        public ActionFormatter(Func<object, string> func) {
            this.func = func;
        }

        public string Format(object value) {
            return func(value);
        }
    }
}
