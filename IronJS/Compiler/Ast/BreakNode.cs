using System;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    // 12.8
    public class BreakNode : Node
    {
        public string Label { get; protected set; }

        public BreakNode(string label)
            : base(NodeType.Break)
        {
            Label = label;
        }

        public override void Print(System.Text.StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.Append(indentStr + "(" + Type);

            if (Label != null)
                writer.Append(" " + Label);

            writer.AppendLine(")");
        }
    
        public override Et Walk(EtGenerator etgen)
        {
            if (Label == null)
                return Et.Break(etgen.FunctionScope.LabelScope.Break());

            return Et.Break(etgen.FunctionScope.LabelScope.Break(Label));
        }
    }
}
