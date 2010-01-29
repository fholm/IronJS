using System;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class ContinueNode : Node
    {
        public string Label { get; protected set; }

        public ContinueNode(string label, ITree node)
            : base(NodeType.Continue, node)
        {
            Label = label;
        }

        public override Et Generate(EtGenerator etgen)
        {
            if (Label == null)
                return Et.Continue(etgen.FunctionScope.LabelScope.Continue());

            return Et.Continue(etgen.FunctionScope.LabelScope.Continue(Label));
        }

        public override void Print(System.Text.StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.Append(indentStr + "(" + Type);

            if (Label != null)
                writer.Append(" " + Label);

            writer.AppendLine(")");
        }
    
    }
}
