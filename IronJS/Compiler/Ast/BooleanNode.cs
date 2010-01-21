using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class BooleanNode : Node
    {
        public readonly bool Value;

        public BooleanNode(bool value)
            : base(NodeType.Boolean)
        {
            Value = value;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Constant(Value, typeof(object));
        }
    }
}
