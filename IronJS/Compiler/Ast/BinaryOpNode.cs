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
                    return JsTypes.Boolean;

                return EvalTypes(Left, Right);
            }
        }

        public override INode Analyze(AstAnalyzer astopt)
        {
            Left = Left.Analyze(astopt);
            Right = Right.Analyze(astopt);

            if (Left is IdentifierNode)
                (Left as IdentifierNode).Variable.AssignedFrom.Add(Right);

            if (Right is IdentifierNode)
                (Right as IdentifierNode).Variable.AssignedFrom.Add(Left);

            return this;
        }

        public override Et GenerateStatic(IjsEtGenerator etgen)
        {
            if (IdenticalTypes(Left, Right))
            {
                var left = Left.GenerateStatic(etgen);
                var right = Right.GenerateStatic(etgen);

                if (Left.ExprType == JsTypes.Integer)
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

        public override Et Generate(EtGenerator etgen)
        {
            return Et.Dynamic(
                etgen.Context.CreateBinaryOpBinder(Op),
                typeof(object),
                Left.Generate(etgen),
                Right.Generate(etgen)
            );
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
