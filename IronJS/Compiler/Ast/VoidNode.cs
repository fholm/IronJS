using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class VoidNode : Node
    {
        public readonly Ast.Node Target;

        public VoidNode(Ast.Node target)
            : base(NodeType.Void)
        {
            Target = target;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Block(
                Target.Walk(etgen),
                Undefined.Expr
            );
        }
    }
}
