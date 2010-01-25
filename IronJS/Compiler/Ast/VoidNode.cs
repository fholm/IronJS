using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{ 
    public class VoidNode : Node
    {
        public Node Target { get; protected set; }

        public VoidNode(Node target)
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
