using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class ContinueNode : Node
    {
        public readonly string Label;

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
    }
}
