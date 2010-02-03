using System;
using System.Text;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;
using IronJS.Runtime2.Js;

namespace IronJS.Compiler.Ast
{
    public class ContinueNode : Node, INode
    {
        public string Label { get; protected set; }

        public ContinueNode(string label, ITree node)
            : base(NodeType.Continue, node)
        {
            Label = label;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.Append(indentStr + "(" + NodeType);

            if (Label != null)
                writer.Append(" " + Label);

            writer.AppendLine(")");
        }
    
    }
}
