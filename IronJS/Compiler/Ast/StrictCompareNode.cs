using System;
using System.Linq.Expressions;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class StrictCompareNode : Node
    {
        public INode Left { get; protected set; }
        public INode Right { get; protected set; }
        public ExpressionType Op { get; protected set; }

        public StrictCompareNode(INode left, INode right, ExpressionType op, ITree node)
            : base(NodeType.StrictCompare, node)
        {
            Left = left;
            Right = right;
            Op = op;
        }

        public override JsType ExprType
        {
            get
            {
                return JsType.Boolean;
            }
        }

        public override Expression Generate(EtGenerator etgen)
        {
            // for both
            Et expr = Et.Call(
                typeof(Operators).GetMethod("StrictEquality"),
                EtUtils.Cast<object>(Left.Generate(etgen)),
                EtUtils.Cast<object>(Right.Generate(etgen))
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
