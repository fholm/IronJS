using System;
using System.Linq.Expressions;
using System.Text;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class BinaryOpNode : Node
    {
        public Node Left { get; protected set; }
        public Node Right { get; protected set; }
        public ExpressionType Op { get; protected set; }

        public BinaryOpNode(Node left, Node right, ExpressionType op, ITree node)
            : base(NodeType.BinaryOp, node)
        {
            Left = left;
            Right = right;
            Op = op;
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
