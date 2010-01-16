using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Utils
{
    static class TypeConverter
    {
        static internal int ToInt32(object obj)
        {
            var dbl = ToNumber(obj);

            if (double.IsNaN(dbl) || double.IsInfinity(dbl))
                return 0;

            return Convert.ToInt32(dbl);
        }

        static internal double ToNumber(object obj)
        {
            if (obj == null)
                return 0.0D;

            if (obj is double)
                return (double)obj;

            if (obj is Js.Undefined)
                return double.NaN;

            if (obj is bool)
                return (bool)obj ? 1.0D : 0.0D;

            if (obj is string)
            {
                double val;

                if (double.TryParse((string)obj, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                    return val;

                return double.NaN;
            }

            if (obj is IObj)
                return ToNumber((obj as IObj).DefaultValue(ValueHint.Number));

            return 1.0D;
        }
    }
}
