using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Parm = System.Linq.Expressions.ParameterExpression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    public static class TypeConverter
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
            {
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
            }

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

            if (obj.LimitType == typeof(Js.Obj))
                return Et.Call(
                    Et.Convert(obj.Expression, typeof(Js.Obj)),
                    typeof(Js.Obj).GetMethod("DefaultValue"),
                    Et.Constant(ValueHint.String)
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
                //TODO: fix so that . instead of , is valid as "3.14" fails atm
                var tmp = Et.Parameter(typeof(double), "#tmp");
                var method = typeof(double).GetMethod("TryParse", new[] { typeof(string), typeof(double).MakeByRefType() });

                return Et.Block(
                    new[] { tmp },
                    Et.Condition(
                        Et.Call(
                            method,
                            Et.Convert(obj.Expression, typeof(string)),
                            tmp
                        ),
                        tmp,
                        Et.Constant(double.NaN, typeof(double))
                    )
                );
            }

            if(obj.LimitType == typeof(Js.Obj))
            {
                return Et.Call(
                    Et.Convert(obj.Expression, typeof(Js.Obj)),
                    typeof(Js.Obj).GetMethod("DefaultValue"),
                    Et.Constant(ValueHint.Number)
                );
            }

            return Et.Constant(1.0, typeof(double));
        }

        public static Et ToObject(Meta obj)
        {
            /*
            if (obj.LimitType == typeof(double))
            {
                var jsObj = new Js.Obj();
                jsObj.Class = ObjClass.Number;

            }

            //TODO: throw TypeError on null/undefined
            if (o is double || o is bool || o is string)
            {
                //TODO: give correct object prototype
                var obj = new Js.Obj();
                obj.Value = o;
                return obj;
            }

            return o;
            */

            return null;
        }
    }
}
