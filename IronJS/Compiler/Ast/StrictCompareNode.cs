using System.Linq.Expressions;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class StrictCompareNode : Node
    {
        public readonly Node Left;
        public readonly Node Right;
        public readonly ExpressionType Op;

        public StrictCompareNode(Node left, Node right, ExpressionType op)
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
                EtUtils.Cast<object>(Left.Walk(etgen)),
                EtUtils.Cast<object>(Right.Walk(etgen))
            );

            // specific to 11.9.5
            if (Op == ExpressionType.NotEqual)
                expr = Et.Not(Et.Convert(expr, typeof(bool)));

            return expr;
        }
    }
}
