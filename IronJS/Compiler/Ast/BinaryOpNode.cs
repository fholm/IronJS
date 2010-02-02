using System;
using System.Linq.Expressions;
using System.Text;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class BinaryOpNode : Node
    {
        public INode Left { get; protected set; }
        public INode Right { get; protected set; }
        public ExpressionType Op { get; protected set; }

        public BinaryOpNode(INode left, INode right, ExpressionType op, ITree node)
            : base(NodeType.BinaryOp, node)
        {
            Op = op;
            Left = left;
            Right = right;
        }

        public bool IsComparisonOp
        {
            get
            {
                return (Op == ExpressionType.LessThan
                    || Op == ExpressionType.LessThanOrEqual
                    || Op == ExpressionType.GreaterThan
                    || Op == ExpressionType.GreaterThanOrEqual
                    || Op == ExpressionType.Equal
                    || Op == ExpressionType.NotEqual);
            }
        }

        public override Type ExprType
        {
            get
            {
                if (IsComparisonOp)
                    return IjsTypes.Boolean;

                return EvalTypes(Left, Right);
            }
        }

        public override INode Analyze(FuncNode astopt)
        {
            Left = Left.Analyze(astopt);
            Right = Right.Analyze(astopt);

            IfIdentifierAssignedFrom(Left, Right);
            IfIdentifierAssignedFrom(Right, Left);

            return this;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Op);

            Left.Print(writer, indent + 1);
            Right.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
