using System;
using System.Linq.Expressions;
using System.Text;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;
using IronJS.Runtime2.Js;

namespace IronJS.Compiler.Ast
{
    public class BinaryOpNode : Node
    {
        public INode Left { get; protected set; }
        public INode Right { get; protected set; }
        public ExpressionType Op { get; protected set; }
        public override Type ExprType { get { return IsComparisonOp ? IjsTypes.Boolean : EvalTypes(Left, Right); } }

        public bool IsComparisonOp
        {
            get
            {
                return (   Op == ExpressionType.LessThan    || Op == ExpressionType.LessThanOrEqual
                        || Op == ExpressionType.GreaterThan || Op == ExpressionType.GreaterThanOrEqual
                        || Op == ExpressionType.Equal       || Op == ExpressionType.NotEqual);
            }
        }

        public BinaryOpNode(INode left, INode right, ExpressionType op, ITree node)
            : base(NodeType.BinaryOp, node)
        {
            Op = op;
            Left = left;
            Right = right;
        }

        public override INode Analyze(FuncNode func)
        {
            Left = Left.Analyze(func);
            Right = Right.Analyze(func);

            IfIdentifierAssignedFrom(Left, Right);
            IfIdentifierAssignedFrom(Right, Left);

            return this;
        }

        public override Et EtGen(FuncNode func)
        {
            if (IdenticalTypes(Left, Right))
            {
                var left = Left.EtGen(func);
                var right = Right.EtGen(func);

                if (Left.ExprType == IjsTypes.Integer)
                {
                    if (Op == ExpressionType.LessThan)
                        return Et.LessThan(left, right);

                    if (Op == ExpressionType.Add)
                        return Et.Add(left, right);
                }
            }
            else
            {

            }

            throw new NotImplementedException();
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
