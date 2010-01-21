using System.Linq.Expressions;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class UnaryOpNode : Node
    {
        public readonly Node Target;
        public readonly ExpressionType Op;

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
