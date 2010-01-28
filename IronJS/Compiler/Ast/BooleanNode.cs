using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class BooleanNode : Node
    {
        public bool Value { get; protected set; }

        public BooleanNode(bool value)
            : base(NodeType.Boolean)
        {
            Value = value;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return etgen.Generate<bool>(Value);
        }
    }
}
