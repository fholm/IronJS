using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using EtParam = System.Linq.Expressions.ParameterExpression;
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    class FunctionTable
    {
        static readonly MethodInfo MiPush = typeof(FunctionTable).GetMethod("Push");
        static readonly MethodInfo MiPull = typeof(FunctionTable).GetMethod("Pull");

        readonly List<Lambda> Functions =
             new List<Lambda>();

        public int Push(Lambda func)
        {
            Functions.Add(func);
            return Functions.Count - 1;
        }

        public Lambda Pull(int i)
        {
            return Functions[i];
        }

        internal static Et EtNew()
        {
            return AstUtils.SimpleNewHelper(
                typeof(FunctionTable).GetConstructor(Type.EmptyTypes)
            );
        }

        internal static Et EtPull(Et table, int n)
        {
            return Et.Call(
                table,
                MiPull,
                Et.Constant(n, typeof(int))
            );
        }

        internal static Et EtPush(Et table, Et lambda)
        {
            return Et.Call(
                table,
                MiPush,
                lambda
            );
        }
    }
}
