using System.Linq.Expressions;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class UnaryOpNode : Node
    {
        public Node Target { get; protected set; }
        public ExpressionType Op { get; protected set; }

        public UnaryOpNode(Node target, ExpressionType op)
            : base(NodeType.UnaryOp)
        {
            Target = target;
            Op = op;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Dynamic(
                etgen.Context.CreateUnaryOpBinder(Op),
                typeof(object),
                Target.Walk(etgen)
            );
        }
    }
}
