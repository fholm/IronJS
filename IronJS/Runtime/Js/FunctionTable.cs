using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    class FunctionTable
    {
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
    }
}
