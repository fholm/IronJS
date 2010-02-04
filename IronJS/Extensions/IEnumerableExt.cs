using System.Collections.Generic;
using System.Linq;
using System.Text;
using INode = IronJS.Compiler.Ast.INode;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;
using System;

namespace IronJS.Extensions
{
    public static class IEnumerableExt
    {
        public static string PrettyPrint(this IEnumerable<INode> that)
        {
            var buffer = new StringBuilder();

            foreach (var str in that.Select(x => x.Print()))
                buffer.AppendLine(str);

            return buffer.ToString();
        }

        public static Et ToBlock<T>(this IEnumerable<T> that, Func<T, Et> transform)
        {
            if (that.Count() == 0)
                return AstUtils.Empty();

            return Et.Block(
                that.Select( x => transform(x))
            );
        }
    }
}
