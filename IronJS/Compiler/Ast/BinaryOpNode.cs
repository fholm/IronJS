using System;
using System.Linq.Expressions;
using System.Text;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class BinaryOpNode : Node
    {
        public readonly Node Left;
        public readonly Node Right;
        public readonly ExpressionType Op;

        public BinaryOpNode(Node left, Node right, ExpressionType op)
            : base(NodeType.BinaryOp)
        {
            Left = left;
            Right = right;
            Op = op;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type + ":" + Op);

            Left.Print(writer, indent + 1);
            Right.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Dynamic(
                etgen.Context.CreateBinaryOpBinder(Op),
                typeof(object),
                Left.Walk(etgen),
                Right.Walk(etgen)
            );
        }
    }
}
