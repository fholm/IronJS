using System.Collections.Generic;
using System.Linq;
using System.Text;
using INode = IronJS.Compiler.Ast.INode;

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
    }
}
