using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class RegexNode : Node
    {
        public string Regex { get; protected set; }
        public string Modifiers { get; protected set; }

        public RegexNode(string regex, ITree node)
            : base(NodeType.Regex, node)
        {
            var lastIndex = regex.LastIndexOf('/');
            Regex = regex.Substring(1, lastIndex - 1);
            Modifiers = regex.Substring(lastIndex + 1);
        }

        public override Et Generate(EtGenerator etgen)
        {
            return Et.Call(
                Et.Constant(etgen.Context),
                Context.MiCreateRegExp,
                Et.Constant(Regex, typeof(object)),
                Et.Constant(Modifiers, typeof(object))
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.Append(indentStr + "(" + Type);
            writer.Append(" " + "/" + Regex + "/" + Modifiers);
            writer.AppendLine(indentStr + ")");
        }
    }
}
