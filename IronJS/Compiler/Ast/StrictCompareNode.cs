using System;
using System.Linq.Expressions;
using System.Text;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class StrictCompareNode : Node
    {
        public Node Left { get; protected set; }
        public Node Right { get; protected set; }
        public ExpressionType Op { get; protected set; }

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

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Op + "Strict");

            Left.Print(writer, indent + 1);
            Right.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
