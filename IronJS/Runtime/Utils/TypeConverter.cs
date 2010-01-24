using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Scripting.Utils;
using IronJS.Runtime.Js;

using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Restrict = System.Dynamic.BindingRestrictions;
using EtParam = System.Linq.Expressions.ParameterExpression;
using IronJS.Extensions;
using System.Globalization;

namespace IronJS.Runtime.Utils
{
    static public class JsTypeConverter
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
                return ((bool)obj) ? true : false;

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

        static public string ToString(object obj)
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

                return dbl.ToString(CultureInfo.InvariantCulture).ToLower();
            }

            if (obj is string)
                return (string)obj;

            return ToString(ToPrimitive(obj, ValueHint.String));
        }

        static internal RegExpObj ToRegExp(object obj, Context context)
        {
            return obj is RegExpObj
                    ? obj as RegExpObj
                    : (RegExpObj)context.RegExpContructor.Construct(new[] { obj });
        }

        static internal IObj ToObject(object obj, Context context)
        {
            if (obj == null || obj is Undefined)
                throw new NotImplementedException("C# throwing of JS exceptions not implemented");

            if (obj is string)
                throw new NotImplementedException();

            if (obj is double)
                throw new NotImplementedException();

            if (obj is bool)
                throw new NotImplementedException();

            if (obj is IObj)
                return ((IObj)obj);

            throw new NotImplementedException("Can't convert host objects to JS objects");
        }


        static public Et EtToBoolean(Meta obj)
        {
            if (obj.HasValue && obj.Value == null)
                return Et.Constant(false, typeof(bool));

            if (obj.LimitType == typeof(bool))
                return Et.Convert(obj.Expression, typeof(bool));

            if (obj.LimitType == typeof(double))
                return Et.GreaterThan(
                    Et.Convert(obj.Expression, typeof(double)),
                    Et.Constant(0.0, typeof(double))
                );

            if (obj.LimitType == typeof(Js.Undefined))
                return Et.Constant(false, typeof(bool));

            if (obj.LimitType == typeof(string))
                return Et.GreaterThan(
                    Et.Property(
                        Et.Convert(
                            obj.Expression,
                            typeof(string)
                        ),
                        "Length"
                    ),
                    Et.Constant(0, typeof(int))
                );

            // last step
            return Et.Constant(true, typeof(bool));
        }

        static public Et EtToString(Meta obj)
        {
            if (obj.LimitType == typeof(string))
                return Et.Convert(obj.Expression, typeof(string));

            if (obj.LimitType == typeof(double))
                return Et.Condition(
                    Et.Call(
                        typeof(double).GetMethod("IsInfinity"),
                        Et.Convert(obj.Expression, typeof(double))
                    ),
                    Et.Constant("Infinity", typeof(string)),
                    Et.Call(
                        obj.Expression,
                        typeof(object).GetMethod("ToString")
                    ),
                    typeof(string)
                );

            if (obj.LimitType == typeof(Js.Undefined))
                return Et.Constant("undefined", typeof(string));

            if (obj.Value == null)
                return Et.Constant("null", typeof(string));

            if (obj.LimitType == typeof(bool))
                return Et.Condition(
                    Et.Convert(obj.Expression, typeof(bool)),
                    Et.Constant("true", typeof(string)),
                    Et.Constant("false", typeof(string)),
                    typeof(string)
                );

            if (typeof(IObj).IsAssignableFrom(obj.LimitType))
                return EtUtils.Cast<string>(
                    Et.Call(
                        Et.Convert(obj.Expression, typeof(Js.IObj)),
                        IObjMethods.MiDefaultValue,
                        Et.Constant(ValueHint.String)
                    )
                );

            // last step, just ToString
            return Et.Call(
                obj.Expression,
                typeof(object).GetMethod("ToString")
            );
        }

        static public Et EtToNumber(Meta obj)
        {
            if (obj.HasValue == true & obj.Value == null)
                return Et.Constant(0.0D, typeof(double));

            //TODO: handle all .NET number types
            if (obj.LimitType == typeof(double))
                return Et.Convert(obj.Expression, typeof(double));

            if (obj.LimitType == typeof(Js.Undefined))
                return Et.Constant(double.NaN, typeof(double));

            if (obj.LimitType == typeof(bool))
                return Et.Condition(
                    Et.Convert(obj.Expression, typeof(bool)),
                    Et.Constant(1.0, typeof(double)),
                    Et.Constant(0.0, typeof(double)),
                    typeof(double)
                );

            if (obj.LimitType == typeof(string))
            {
                var tmp = Et.Parameter(typeof(double), "#tmp");

                var method = typeof(double).GetMethod(
                        "TryParse",
                        new[] { 
                            typeof(string), 
                            typeof(NumberStyles),
                            typeof(IFormatProvider),
                            typeof(double).MakeByRefType()
                        }
                    );

                return Et.Block(
                    new[] { tmp },
                    Et.Condition(
                        Et.Call(
                            method,
                            Et.Convert(obj.Expression, typeof(string)),
                            Et.Constant(NumberStyles.Any, typeof(NumberStyles)),
                            Et.Constant(CultureInfo.InvariantCulture, typeof(IFormatProvider)),
                            tmp
                        ),
                        tmp,
                        Et.Constant(double.NaN, typeof(double))
                    )
                );
            }

            if (typeof(IObj).IsAssignableFrom(obj.LimitType))
                return EtUtils.Cast<double>(
                    Et.Call(
                        Et.Convert(obj.Expression, typeof(Js.IObj)),
                        IObjMethods.MiDefaultValue,
                        Et.Constant(ValueHint.Number)
                    )
                );

            return Et.Constant(1.0, typeof(double));
        }

        static public Et EtToObject(Meta obj, Context context)
        {
            if (obj.LimitType == typeof(Js.Undefined) || (obj.HasValue && obj.Value == null))
                throw new NotImplementedException("Need do handle null/undefined in ToObject");

            if (obj.LimitType == typeof(double))
                return Et.Call(
                    Et.Constant(context, typeof(Context)),
                    Context.MiCreateNumber,
                    Et.Convert(obj.Expression, typeof(object))
                );

            if (obj.LimitType == typeof(string))
                return Et.Call(
                    Et.Constant(context, typeof(Context)),
                    Context.MiCreateString,
                    Et.Convert(obj.Expression, typeof(object))
                );

            if (obj.LimitType == typeof(bool))
                return Et.Call(
                    Et.Constant(context, typeof(Context)),
                    Context.MiCreateBoolean,
                    Et.Convert(obj.Expression, typeof(object))
                );

            return obj.Expression;
        }

    }
}
