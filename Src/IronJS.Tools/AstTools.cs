using System;
using System.Dynamic;
using System.Collections.Generic;

#if CLR2
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Tools
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;

    public static class AstTools
    {
        public static Et BuildBlock<T>(IEnumerable<T> collection, Func<T, Et> filter)
        {
            Et[] expressions = IEnumerableTools.Map(collection, filter);

            if (expressions.Length == 0)
                return AstUtils.Empty();

            return Et.Block(expressions);
        }
    }
}
