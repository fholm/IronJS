using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using Restrict = System.Dynamic.BindingRestrictions;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime.Utils
{
    static class EtUtils
    {
        internal static Et Box(Et expr)
        {
            if (!expr.Type.IsValueType)
                return expr;

            if (expr.Type == typeof(void))
                return Et.Block(expr, Et.Default(typeof(object)));

            return Et.Convert(expr, typeof(object));
        }

        internal static Et Box2(Et expr)
        {
            if (expr.Type == typeof(void))
                return Et.Block(expr, Et.Default(typeof(object)));

            return Et.Convert(expr, typeof(object));
        }

        internal static Et CastForBitOp(Et expr)
        {
            // we need to go object > double > int
            // instead of object > int (which fill fail)
            return Et.Convert(
                Et.Convert(
                    expr,
                    typeof(double)
                ),
                typeof(int)
            );
        }

        internal static Et Cast<T>(Et expr)
        {
            if (expr.Type == typeof(void))
                return Et.Block(expr, Et.Default(typeof(T)));

            return Et.Convert(expr, typeof(T));
        }

        internal static Meta CreateThrow(Meta target, Meta[] args, Restrict tests, Type excType, params object[] excArgs)
        {
            Et[] argExprs = null;
            Type[] argTypes = Type.EmptyTypes;

            if (excArgs != null)
            {
                var i = excArgs.Length;

                argExprs = new Et[i];
                argTypes = new Type[i];

                i = 0;

                foreach (object o in excArgs)
                {
                    Et expr = Et.Constant(o);

                    argExprs[i] = expr;
                    argTypes[i] = expr.Type;

                    i += 1;
                }
            }

            var constructor = excType.GetConstructor(argTypes);

            if (constructor == null)
                throw InternalRuntimeError.New("Exception doesn't have a matching constructor");

            return new Meta(
                Et.Throw(
                    Et.New(constructor, argExprs),
                    typeof(object)
                ),
                target.Restrictions.Merge(
                    Restrict.Combine(args).Merge(
                        tests
                    )
                )
            );
        }

        internal static Et[] ConvertAll<T>(Meta[] args)
        {
            Et[] callArgs = new Et[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                callArgs[i] =
                    Et.Convert(
                        args[i].Expression,
                        typeof(T)
                    );
            }

            return callArgs;
        }

        internal static Et[] ConvertToParamTypes(IEnumerable<Et> args, ParameterInfo[] paramInfo)
        {
            Et[] converted = new Et[args.Count()];

            var n = 0;

            foreach (var arg in args)
            {
                converted[n] = Et.Convert(
                    arg,
                    paramInfo[n].ParameterType
                );
                ++n;
            }

            return converted;
        }

        internal static Et[] ConvertToParamTypes(IEnumerable<Meta> args, ParameterInfo[] paramInfo)
        {
            return ConvertToParamTypes(
                args.Select(x => x.Expression), 
                paramInfo
            );
        }


        internal static Et CreateBlockIfNotEmpty(IEnumerable<Et> exprs)
        {
            if (exprs.Count() > 0)
                return Et.Block(exprs);

            return AstUtils.Empty();
        }
    }
}
