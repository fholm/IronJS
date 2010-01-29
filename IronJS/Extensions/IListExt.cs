using System;
using System.Collections.Generic;
using System.Linq;
using Et = System.Linq.Expressions.Expression;
using Node = IronJS.Compiler.Ast.INode;

namespace IronJS.Extensions
{
    static class IListExt
    {
        public static Et[] ToEtArray(this IList<Node> that, Func<Node, Et> func)
        {
            return that.Select(func).ToArray();
        }
    }
}
