using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{

    class NullNode : Node
    {
        public NullNode()
            : base(NodeType.Null)
        {

        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Default(typeof(object));
        }
    }
}
