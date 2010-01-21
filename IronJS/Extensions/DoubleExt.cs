using System.Globalization;

namespace IronJS.Extensions
{
    static class DoubleExt
    {
        /// <summary>
        /// Same as double.tryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal static bool TryParseInvariant(string s, out double result)
        {
            return double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }
    }
}
