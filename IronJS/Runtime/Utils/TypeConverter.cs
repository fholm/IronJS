using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using IronJS.Runtime.Js;
using IronJS.Extensions;

namespace IronJS.Runtime.Utils
{
    static class TypeConverter
    {
        static internal object ToPrimitive(object obj, ValueHint hint = ValueHint.None)
        {
            if (obj == null
               || obj is Js.Undefined
               || obj is bool
               || obj is double
               || obj is string)
                return obj;

            if (obj is IObj)
                return ((IObj)obj).DefaultValue(hint);

            return obj.ToString();
        }

        static internal bool ToBoolean(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Js.Undefined)
                return false;

            if (obj is bool)
                return true;

            if (obj is double)
                return Convert.ToBoolean((double)obj);

            if (obj is string)
                return ((string)obj).Length > 0;

            return true;
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
                double result;

                if (DoubleExt.TryParseInvariant((string)obj, out result))
                    return result;

                return double.NaN;
            }

            if (obj is IObj)
                return ToNumber(ToPrimitive(obj, ValueHint.Number));

            return Convert.ToDouble(obj);
        }

        static internal int ToInteger(object obj)
        {
            return ToInt32(obj);
        }

        static internal int ToInt32(object obj)
        {
            var dbl = ToNumber(obj);

            if (double.IsNaN(dbl) || double.IsInfinity(dbl))
                return 0;

            return Convert.ToInt32(dbl);
        }

        static internal uint ToUInt32(object obj)
        {
            var dbl = ToNumber(obj);

            if (double.IsNaN(dbl) || double.IsInfinity(dbl))
                return 0;

            return Convert.ToUInt32(dbl);
        }

        static internal ushort ToUInt16(object obj)
        {
            var dbl = ToNumber(obj);

            if (double.IsNaN(dbl) || double.IsInfinity(dbl))
                return 0;

            return Convert.ToUInt16(dbl);
        }

        static internal string ToString(object obj)
        {
            if (obj == null)
                return "null";

            if (obj is Js.Undefined)
                return "undefined";

            if (obj is bool)
                return ((bool)obj) ? "true" : "false";

            if (obj is double)
            {
                var dbl = (double)obj;

                if (double.IsNaN(dbl))
                    return "NaN";

                if (double.IsInfinity(dbl))
                    return "Infinity";

                return dbl.ToString(CultureInfo.InvariantCulture);
            }

            if (obj is string)
                return (string)obj;

            return ToString(ToPrimitive(obj, ValueHint.String));
        }

        static internal IObj ToObject(object obj, Context context)
        {
            if (obj == null || obj is Undefined)
                throw new NotImplementedException("C# throwing of JS exceptions not implemented");

            if (obj is string)
                context.CreateString((string)obj);

            if (obj is double)
                context.CreateNumber((double)obj);

            if (obj is bool)
                context.CreateBoolean((bool)obj);

            if (obj is IObj)
                return ((IObj)obj);

            throw new NotImplementedException("Can't convert host objects to JS objects");
        }
    }
}
