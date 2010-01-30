using System;
using System.Linq;
using System.Collections.Generic;
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

            return JsTypes.Dynamic;
        }
    }
}
