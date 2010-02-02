using System;
using System.Collections.Generic;
using System.Linq;
using IronJS.Compiler;
using IronJS.Compiler.Ast;

namespace IronJS.Extensions
{
    public static class HashSetExt
    {
        public static Type EvalType(this HashSet<Type> set)
        {
            set.Remove(null);

            if (set.Count == 1)
                return set.First();

            return IjsTypes.Dynamic;
        }

        public static Type EvalType(this HashSet<INode> set)
        {
            set.Remove(null);

            return new HashSet<Type>(
                set.Select(x => x.ExprType)
            ).EvalType();
        }
    }
}
