using System;
using System.Text;

namespace IronJS.Compiler.Ast
{
    using Et = System.Linq.Expressions.Expression;

    enum BinaryOp { Eq, Lt, Add, Sub, Mul, Div, Mod, Gt, GtEq, LtEq, NotEq, EqEq, NotEqEq, Inc, ShiftLeft, ShiftRight, ShiftRightZero, And, Or, Xor, LogicalOr, LogicalAnd }

    class BinaryOpNode : Node
    {
        public readonly Node Left;
        public readonly Node Right;
        public readonly BinaryOp Op;

        public BinaryOpNode(Node left, Node right, BinaryOp op)
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
            throw new NotImplementedException();
        }
    }
}
