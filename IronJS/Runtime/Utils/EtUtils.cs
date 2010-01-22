using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Scripting.Utils;
using IronJS.Runtime.Js;

using AstUtils = Microsoft.Scripting.Ast.Utils;
using EtParam = System.Linq.Expressions.ParameterExpression;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using Restrict = System.Dynamic.BindingRestrictions;
using System.Reflection;

namespace IronJS.Runtime.Utils
{
    static class EtUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        internal static Et Box(Et expr)
        {
            if (!expr.Type.IsValueType)
                return expr;

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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        internal static Et Cast<T>(Et expr)
        {
            if (expr.Type == typeof(void))
                return Et.Block(expr, Et.Default(typeof(T)));

            return Et.Convert(expr, typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        /// <param name="tests"></param>
        /// <param name="excType"></param>
        /// <param name="excArgs"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="paramInfo"></param>
        /// <returns></returns>
        internal static Et[] ConvertToParamTypes(Meta[] args, ParameterInfo[] paramInfo)
        {
            Et[] callArgs = new Et[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                callArgs[i] =
                    Et.Convert(
                        args[i].Expression,
                        paramInfo[i].ParameterType
                    );
            }

            return callArgs;
        }


        internal static Et CreateBlockIfNotEmpty(IEnumerable<Et> exprs)
        {
            if (exprs.Count() > 0)
                return Et.Block(exprs);

            return AstUtils.Empty();
        }
    }
}
