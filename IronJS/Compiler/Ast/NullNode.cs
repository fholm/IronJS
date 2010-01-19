
namespace IronJS.Compiler.Ast
{
    using Et = System.Linq.Expressions.Expression;

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
