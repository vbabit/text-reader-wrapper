using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DevSpatium.IO.TextReader
{
    internal static class Helpers
    {
        public static ConfiguredTaskAwaitable NonUi(this Task task)
            => task.NotNull(nameof(task)).ConfigureAwait(false);

        public static ConfiguredTaskAwaitable<T> NonUi<T>(this Task<T> task)
            => task.NotNull(nameof(task)).ConfigureAwait(false);

        public static string ToInvariantString(this object value)
            => value.ToInvariantString("{0}");

        public static string ToInvariantString(this object value, string format)
            => string.Format(CultureInfo.InvariantCulture, format, value ?? "<null>");

        public static string ToInvariantString(string format, params object[] arguments)
            => string.Format(CultureInfo.InvariantCulture, format, arguments);

        public static bool TryParseToIntInvariant(string value, out int result)
            => int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
    }
}