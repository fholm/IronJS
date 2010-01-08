using System;
using System.Reflection;

namespace IronJS.Runtime.Utils
{
    using Et2 = System.Linq.Expressions.Expression;
    using Meta2 = System.Dynamic.DynamicMetaObject;
    using Restrict2 = System.Dynamic.BindingRestrictions;

    static class EtUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        internal static Et2 Box(Et2 expr)
        {
            if (!expr.Type.IsValueType)
                return expr;

            if (expr.Type == typeof(void))
                return Et2.Block(expr, Et2.Default(typeof(object)));

            return Et2.Convert(expr, typeof(object));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        internal static Et2 Cast<T>(Et2 expr)
        {
            if (expr.Type.IsValueType)
                return expr;

            if (expr.Type == typeof(void))
                return Et2.Block(expr, Et2.Default(typeof(T)));

            return Et2.Convert(expr, typeof(T));
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
        internal static Meta2 CreateThrow(Meta2 target, Meta2[] args, Restrict2 tests, Type excType, params object[] excArgs)
        {
            Et2[] argExprs = null;
            Type[] argTypes = Type.EmptyTypes;

            if (excArgs != null)
            {
                var i = excArgs.Length;

                argExprs = new Et2[i];
                argTypes = new Type[i];

                i = 0;

                foreach (object o in excArgs)
                {
                    Et2 expr = Et2.Constant(o);

                    argExprs[i] = expr;
                    argTypes[i] = expr.Type;

                    i += 1;
                }
            }

            var constructor = excType.GetConstructor(argTypes);

            if (constructor == null)
                throw new Runtime.RuntimeError("Exception '{0}' doesn't have a matching constructor", excType.Name);

            return new Meta2(
                Et2.Throw(
                    Et2.New(constructor, argExprs),
                    typeof(object)
                ),
                target.Restrictions.Merge(
                    Restrict2.Combine(args).Merge(
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
        internal static Et2[] ConvertAll<T>(Meta2[] args)
        {
            Et2[] callArgs = new Et2[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                callArgs[i] =
                    Et2.Convert(
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
        internal static Et2[] ConvertToParamTypes(Meta2[] args, ParameterInfo[] paramInfo)
        {
            Et2[] callArgs = new Et2[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                callArgs[i] =
                    Et2.Convert(
                        args[i].Expression,
                        paramInfo[i].ParameterType
                    );
            }

            return callArgs;
        }

    }
}
