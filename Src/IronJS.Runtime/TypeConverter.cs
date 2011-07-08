using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace IronJS.Runtime
{
    public class TypeConverter
    {
        public static BoxedValue ToBoxedValue(BoxedValue v)
        {
            return v;
        }

        public static BoxedValue ToBoxedValue(double d)
        {
            return BoxedValue.Box(d);
        }

        public static BoxedValue ToBoxedValue(bool b)
        {
            return BoxedValue.Box(b);
        }

        public static BoxedValue ToBoxedValue(string s)
        {
            return BoxedValue.Box(s);
        }

        public static BoxedValue ToBoxedValue(SuffixString s)
        {
            return BoxedValue.Box(s);
        }

        public static BoxedValue ToBoxedValue(CommonObject o)
        {
            return BoxedValue.Box(o);
        }

        public static BoxedValue ToBoxedValue(FunctionObject f)
        {
            return BoxedValue.Box(f);
        }

        public static BoxedValue ToBoxedValue(Undefined u)
        {
            return Undefined.Boxed;
        }

        public static BoxedValue ToBoxedValue(object c)
        {
            return BoxedValue.Box(c);
        }

        public static object ToClrObject(double d)
        {
            return d;
        }

        public static object ToClrObject(bool b)
        {
            return b;
        }

        public static object ToClrObject(string s)
        {
            return s;
        }

        public static object ToClrObject(CommonObject o)
        {
            return o;
        }

        public static object ToClrObject(FunctionObject f)
        {
            return f;
        }

        public static object ToClrObject(object c)
        {
            return c;
        }

        public static object ToClrObject(BoxedValue v)
        {
            switch (v.Tag)
            {
                case TypeTags.Undefined:
                    return null;

                case TypeTags.Bool:
                    return v.Bool;

                case TypeTags.Object:
                case TypeTags.Function:
                case TypeTags.String:
                case TypeTags.Clr:
                    return v.Clr;

                case TypeTags.SuffixString:
                    return v.Clr.ToString();

                default:
                    return v.Number;
            }
        }

        public static CommonObject ToObject(Environment env, CommonObject o)
        {
            return o;
        }

        public static CommonObject ToObject(Environment env, FunctionObject f)
        {
            return f;
        }

        public static CommonObject ToObject(Environment env, Undefined undef)
        {
            return env.RaiseTypeError<CommonObject>("Can't convert Undefined to Object");
        }

        public static CommonObject ToObject(Environment env, object o)
        {
            return env.RaiseTypeError<CommonObject>("Can't convert Null or CLR to Object");
        }

        public static CommonObject ToObject(Environment env, string s)
        {
            return env.NewString(s);
        }

        public static CommonObject ToObject(Environment env, double n)
        {
            return env.NewNumber(n);
        }

        public static CommonObject ToObject(Environment env, bool b)
        {
            return env.NewBoolean(b);
        }

        public static CommonObject ToObject(Environment env, BoxedValue v)
        {
            switch (v.Tag)
            {
                case TypeTags.Object:
                case TypeTags.Function:
                    return v.Object;

                case TypeTags.SuffixString:
                    return env.NewString(v.Clr.ToString());

                case TypeTags.String:
                    return env.NewString(v.String);

                case TypeTags.Bool:
                    return env.NewBoolean(v.Bool);

                case TypeTags.Clr:
                case TypeTags.Undefined:
                    return env.RaiseTypeError<CommonObject>("Can't convert Undefined, Null or CLR to Object");

                default:
                    return env.NewNumber(v.Number);
            }
        }

        public static bool ToBoolean(bool b)
        {
            return b;
        }

        public static bool ToBoolean(double d)
        {
            return ((d > 0.0) || (d < 0.0));
        }

        public static bool ToBoolean(object c)
        {
            return c != null;
        }

        public static bool ToBoolean(string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        public static bool ToBoolean(Undefined u)
        {
            return false;
        }

        public static bool ToBoolean(CommonObject o)
        {
            return true;
        }

        public static bool ToBoolean(BoxedValue v)
        {
            switch (v.Tag)
            {
                case TypeTags.Bool:
                    return v.Bool;

                case TypeTags.String:
                    return !string.IsNullOrEmpty(v.String);

                case TypeTags.SuffixString:
                    var ss = (SuffixString)v.Clr;
                    return ss.Length > 0;

                case TypeTags.Undefined:
                    return false;

                case TypeTags.Clr:
                    return v.Clr != null;

                case TypeTags.Object:
                case TypeTags.Function:
                    return true;

                default:
                    return ToBoolean(v.Number);
            }
        }

        public static BoxedValue ToPrimitive(bool b, DefaultValueHint hint)
        {
            return BoxedValue.Box(b);
        }

        public static BoxedValue ToPrimitive(double d, DefaultValueHint hint)
        {
            return BoxedValue.Box(d);
        }

        public static BoxedValue ToPrimitive(string s, DefaultValueHint hint)
        {
            return BoxedValue.Box(s);
        }

        public static BoxedValue ToPrimitive(CommonObject o, DefaultValueHint hint)
        {
            return o.DefaultValue(hint);
        }

        public static BoxedValue ToPrimitive(Undefined u, DefaultValueHint hint)
        {
            return Undefined.Boxed;
        }

        public static BoxedValue ToPrimitive(object c, DefaultValueHint hint)
        {
            if (c == null)
            {
                return BoxedValue.Box(default(object));
            }

            return BoxedValue.Box(c.ToString());
        }

        public static BoxedValue ToPrimitive(BoxedValue v)
        {
            return ToPrimitive(v, DefaultValueHint.None);
        }

        public static BoxedValue ToPrimitive(BoxedValue v, DefaultValueHint hint)
        {
            switch (v.Tag)
            {
                case TypeTags.Clr:
                    return ToPrimitive(v.Clr, hint);

                case TypeTags.Object:
                case TypeTags.Function:
                    return v.Object.DefaultValue(hint);

                case TypeTags.SuffixString:
                    return BoxedValue.Box(v.Clr.ToString());

                default:
                    return v;
            }
        }

        public static string ToString(bool b)
        {
            return b ? "true" : "false";
        }

        public static string ToString(string s)
        {
            return s;
        }

        public static string ToString(Undefined u)
        {
            return "undefined";
        }

        public static string ToString(object c)
        {
            return c == null ? "null" : c.ToString();
        }

        /// These steps are outlined in the ECMA-262, Section 9.8.1
        public static string ToString(double m)
        {
            if (double.IsNaN(m))
            {
                return "NaN";
            }

            if (m == 0.0)
            {
                return "0";
            }

            string sign = (m >= 0.0) ? "" : "-";

            m = (m >= 0.0) ? m : -m;

            if (double.IsInfinity(m))
            {
                return sign + "Infinity";
            }

            var format = "0.00000000000000000e0";
            var parts = m.ToString(format, CultureInfo.InvariantCulture).Split('e');
            var s = parts[0].TrimEnd('0').Replace(".", "");
            int k = s.Length;
            int n = int.Parse(parts[1]) + 1;

            if (k <= n && n <= 21)
            {
                return sign + s + new string('0', n - k);
            }
            else if (0 < n && n <= 21)
            {
                return sign + s.Substring(0, n) + "." + s.Substring(n);
            }
            else if (-6 < n && n <= 0)
            {
                return sign + "0." + new string('0', -n) + s;
            }

            var exponent = "e" + string.Format("{0:+0;-0}", n - 1);
            return k == 1
                ? sign + s + exponent
                : sign + s.Substring(0, 1) + "." + s.Substring(1) + exponent;
        }

        public static string ToString(CommonObject o)
        {
            var s = o as StringObject;
            return s != null
                ? s.Value.Value.String
                : ToString(o.DefaultValue(DefaultValueHint.String));
        }

        public static string ToString(BoxedValue v)
        {
            switch (v.Tag)
            {
                case TypeTags.Bool:
                    return ToString(v.Bool);

                case TypeTags.String:
                    return v.String;

                case TypeTags.SuffixString:
                    return v.Clr.ToString();

                case TypeTags.Clr:
                    return ToString(v.Clr);

                case TypeTags.Undefined:
                    return "undefined";

                case TypeTags.Object:
                case TypeTags.Function:
                    return ToString(v.Object);

                default:
                    return ToString(v.Number);
            }
        }

        public static double ToNumber(bool b)
        {
            return b ? 1.0 : 0.0;
        }

        public static double ToNumber(object c)
        {
            return c != null ? 1.0 : 0.0;
        }

        public static double ToNumber(Undefined u)
        {
            return double.NaN;
        }

        public static double ToNumber(BoxedValue v)
        {
            if (v.Marker >= Markers.Tagged)
            {
                switch (v.Tag)
                {
                    case TypeTags.Bool:
                        return ToNumber(v.Bool);

                    case TypeTags.String:
                        return ToNumber(v.String);

                    case TypeTags.SuffixString:
                        return ToNumber(v.Clr.ToString());

                    case TypeTags.Clr:
                        return ToNumber(v.Clr);

                    case TypeTags.Undefined:
                        return double.NaN;

                    case TypeTags.Object:
                    case TypeTags.Function:
                        return ToNumber(v.Object);
                }
            }

            return v.Number;
        }

        public static double ToNumber(FunctionObject f)
        {
            return ToNumber(f.DefaultValue(DefaultValueHint.Number));
        }

        public static double ToNumber(CommonObject o)
        {
            var n = o as NumberObject;
            return n != null
                ? n.Value.Value.Number
                : ToNumber(o.DefaultValue(DefaultValueHint.Number));
        }

        public static double ToNumber(string s)
        {
            double d;

            s = s == null ? null : s.Trim();
            if (string.IsNullOrEmpty(s))
            {
                return 0.0;
            }

            if (string.Equals(s, "+Infinity"))
            {
                return double.PositiveInfinity;
            }

            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out d) && !s.Contains(","))
            {
                if (d != 0.0)
                {
                    return d;
                }

                return s[0] == '-' ? -0.0 : 0.0;
            }
            else if ((s.Length > 1 && s[0] == '0') && (s[1] == 'x' || s[1] == 'X'))
            {
                int i;
                if (int.TryParse(s.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out i))
                {
                    return i;
                }

                return double.NaN;
            }

            try
            {
                return Convert.ToInt32(s, 8);
            }
            catch (FormatException) { }
            catch (ArgumentException) { }
            catch (OverflowException) { }

            BigInteger bi;
            bool success;
#if LEGACY_BIGINT
            success = BigIntegerParser.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out bi);
#else
            success = BigInteger.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out bi);
#endif

            if (success && !s.Contains(","))
            {
                return double.PositiveInfinity;
            }

            return double.NaN;
        }

        public static double ToNumber(double d)
        {
            if (double.IsNaN(d) && (TaggedBools.True == BitConverter.DoubleToInt64Bits(d)))
            {
                return 1.0;
            }

            if (double.IsNaN(d) && (TaggedBools.False == BitConverter.DoubleToInt64Bits(d)))
            {
                return 0.0;
            }

            return d;
        }

        public static int ToInt32(double d)
        {
            return (int)(uint)d;
        }

        public static int ToInt32(BoxedValue b)
        {
            return (int)(uint)ToNumber(b);
        }

        public static uint ToUInt32(double d)
        {
            return (uint)d;
        }

        public static uint ToUInt32(BoxedValue b)
        {
            return (uint)ToNumber(b);
        }

        public static ushort ToUInt16(double d)
        {
            return (ushort)(uint)d;
        }

        public static ushort ToUInt16(BoxedValue b)
        {
            return (ushort)(uint)ToNumber(b);
        }

        public static int ToInteger(double d)
        {
            if (d > 0x7fffffff)
            {
                return 0x7fffffff;
            }

            return (int)(uint)d;
        }

        public static int ToInteger(BoxedValue b)
        {
            return ToInteger(ToNumber(b));
        }

        public static bool TryToIndex(double value, out uint index)
        {
            index = (uint)value;
            return ((double)index) == value;
        }

        public static bool TryToIndex(string value, out uint index)
        {
            return uint.TryParse(value, out index);
        }

        public static bool TryToIndex(BoxedValue value, out uint index)
        {
            if (value.IsNumber) return TryToIndex(value.Number, out index);
            if (value.IsString) return TryToIndex(value.String, out index);

            index = default(uint);
            return false;
        }
    }
}
