using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Extensions
{
    using Et = System.Linq.Expressions.Expression;
    using Node = Compiler.Ast.Node;

    static class IListExt
    {
        public static Et[] ToEtArray(this IList<Node> that, Func<Node, Et> func)
        {
            return that.Select(func).ToArray();
        }
    }
}
