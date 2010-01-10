using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Js
{
    public static class TypeConverter
    {
        public static object ToPrimitive(object o, ToPrimitiveHint hint)
        {
            if (o is Js.Obj)
                (o as Js.Obj).DefaultValue(hint);

            if (o is string || o is double || o is bool || o == Js.Undefined.Instance)
                return o;

            return o.ToString();
        }

        public static object ToBoolean(object o)
        {
            if (o is double)
                return ((double)o) > 0.0;

            if (o == Js.Undefined.Instance || o == null)
                return false;

            if (o is string)
                return ((string)o).Length > 0;

            return true;
        }

        public static object ToNumber(object o)
        {
            //TODO: handle all .NET number types
            if (o is double || o is int)
                return (double)o;

            if (o == Js.Undefined.Instance)
                return Js.Nan.Instance;

            if (o is bool)
                return ((bool)o) ? 1.0 : +0.0;

            if (o is string)
            {
                double val;

                if (double.TryParse(o.ToString(), out val))
                    return val;

                return Js.Nan.Instance;
            }

            return ToNumber(ToPrimitive(o, ToPrimitiveHint.Number));
        }

        public static object ToInteger(object v)
        {
            //TODO: implement ToInt32, ToUInt32, ToUint16
            var o = ToNumber(v);

            if (o == Js.Nan.Instance)
                return +0;

            var n = (double)o;

            if (n == 0.0 || double.IsNegativeInfinity(n) || double.IsPositiveInfinity(n))
                return n;

            return Math.Sign(n) * Math.Floor(Math.Abs(n));
        }

        public static object ToString(object o)
        {
            if (o is string)
                return o;

            if (o is double)
            {
                var n = (double)o;

                if (double.IsInfinity(n))
                    return "Infinity";

                return n.ToString();
            }

            if (o == Js.Undefined.Instance)
                return "undefined";

            if (o == Js.Nan.Instance)
                return "NaN";

            if (o == null)
                return "null";

            if (o is bool)
                return ((bool)o) ? "true" : "false";

            // last step, just ToString
            return ToString(ToPrimitive(o, ToPrimitiveHint.String));
        }

        public static object ToObject(object o)
        {
            //TODO: throw TypeError on null/undefined
            if (o is double || o is bool || o is string)
            {
                //TODO: give correct object prototype
                var obj = new Js.Obj();
                obj.Value = o;
                return obj;
            }

            return o;
        }
    }
}
