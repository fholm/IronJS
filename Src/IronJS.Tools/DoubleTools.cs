using System.Globalization;

namespace IronJS.Tools
{
    public static class DoubleTools
    {
        public static bool TryParseInvariant(string s, out double result)
        {
            return double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }
    }
}
