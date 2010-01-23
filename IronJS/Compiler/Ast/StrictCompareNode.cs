using System.Linq.Expressions;
using IronJS.Runtime;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class StrictCompareNode : Node
    {
        public readonly Ast.Node Left;
        public readonly Ast.Node Right;
        public readonly ExpressionType Op;

        public StrictCompareNode(Ast.Node left, Ast.Node right, ExpressionType op)
            : base(NodeType.StrictCompare)
        {
            Left = left;
            Right = right;
            Op = op;
        }

        public override Expression Walk(EtGenerator etgen)
        {
            // for both
            Et expr = Et.Call(
                typeof(Operators).GetMethod("StrictEquality"),
                Left.Walk(etgen),
                Right.Walk(etgen)
            );

            // specific to 11.9.5
            if (Op == ExpressionType.NotEqual)
                expr = Et.Not(Et.Convert(expr, typeof(bool)));

            return expr;
        }
    }
}
