using System;
using System.Linq.Expressions;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class LogicalNode : Node
    {
        public INode Left { get; protected set; }
        public INode Right { get; protected set; }
        public ExpressionType Op { get; protected set; }

        public LogicalNode(INode left, INode right, ExpressionType op, ITree node)
            : base(NodeType.Logical, node)
        {
            Left = left;
            Right = right;
            Op = op;
        }

        public override Type ExprType
        {
            get
            {
                if (Left.ExprType == Right.ExprType)
                    return Left.ExprType;

                return IjsTypes.Dynamic;
            }
        }

        public override INode Analyze(FuncNode astopt)
        {
            Left = Left.Analyze(astopt);
            Right = Right.Analyze(astopt);
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
