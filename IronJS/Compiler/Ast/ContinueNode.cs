using Et = System.Linq.Expressions.Expression;
using System;

namespace IronJS.Compiler.Ast
{
    public class ContinueNode : Node
    {
        public string Label { get; protected set; }

        public ContinueNode()
            : this(null)
        {

        }

        public ContinueNode(string label)
            : base(NodeType.Continue)
        {
            Label = label;
        }

        public override Et Walk(EtGenerator etgen)
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
