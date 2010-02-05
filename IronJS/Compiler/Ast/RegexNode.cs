using System;
using System.Text;
using Antlr.Runtime.Tree;

namespace IronJS.Compiler.Ast
{
    public class RegexNode : Node
    {
        public string Regex { get; protected set; }
        public string Modifiers { get; protected set; }

        public RegexNode(string regex, ITree node)
            : base(NodeType.Regex, node)
        {
            int lastIndex = regex.LastIndexOf('/');
            Regex = regex.Substring(1, lastIndex - 1);
            Modifiers = regex.Substring(lastIndex + 1);
        }

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.Append(indentStr + "(" + NodeType);
            writer.Append(" " + "/" + Regex + "/" + Modifiers);
            writer.AppendLine(indentStr + ")");
        }
    }
}
