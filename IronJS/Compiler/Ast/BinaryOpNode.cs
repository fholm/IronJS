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

        public override JsType ExprType
        {
            get
            {
                return EvalTypes(Left, Right);
            }
        }

        public override INode Optimize(AstOptimizer astopt)
        {
            Left = Left.Optimize(astopt);
            Right = Right.Optimize(astopt);

            if (Left is IdentifierNode)
                (Left as IdentifierNode).Variable.AssignedFrom.Add(Right);

            if (Right is IdentifierNode)
                (Right as IdentifierNode).Variable.AssignedFrom.Add(Left);

            return this;
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
