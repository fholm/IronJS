using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Js.Utils
{
    using Et = System.Linq.Expressions.Expression;
    using Parm = System.Linq.Expressions.ParameterExpression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using System.Globalization;

    public static class EtTypeConverter
    {
        public static Et ToBoolean(Meta obj)
        {
            if(obj.HasValue && obj.Value == null)
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

        public static Et ToString(Meta obj)
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

            if(obj.LimitType == typeof(Js.Undefined))
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
                        typeof(Js.IObj).GetMethod("DefaultValue"),
                        Et.Constant(ValueHint.String)
                    )
                );

            // last step, just ToString
            return Et.Call(
                obj.Expression,
                typeof(object).GetMethod("ToString")
            );
        }

        public static Et ToNumber(Meta obj)
        {
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

            if(typeof(IObj).IsAssignableFrom(obj.LimitType))
                return EtUtils.Cast<double>(
                    Et.Call(
                        Et.Convert(obj.Expression, typeof(Js.IObj)),
                        typeof(Js.IObj).GetMethod("DefaultValue"),
                        Et.Constant(ValueHint.Number)
                    )
                );

            return Et.Constant(1.0, typeof(double));
        }

        public static Et ToObject(Meta obj, Context context)
        {
            if (obj.LimitType == typeof(Js.Undefined) || (obj.HasValue && obj.Value == null))
                throw new NotImplementedException("Need do handle null/undefined in ToObject");

            if (obj.LimitType == typeof(double))
                return Et.Call(
                    Et.Constant(context, typeof(Context)),
                    Context.Methods.CreateNumber,
                    Et.Convert(obj.Expression, typeof(double))
                );

            if (obj.LimitType == typeof(string))
                return Et.Call(
                    Et.Constant(context, typeof(Context)),
                    Context.Methods.CreateString,
                    Et.Convert(obj.Expression, typeof(string))
                );

            if (obj.LimitType == typeof(bool))
                return Et.Call(
                    Et.Constant(context, typeof(Context)),
                    Context.Methods.CreateBoolean,
                    Et.Convert(obj.Expression, typeof(bool))
                );

            return obj.Expression;
        }
    }
}
