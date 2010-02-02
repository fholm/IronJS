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

        public override Type ExprType
        {
            get
            {
                return IjsTypes.Boolean;
            }
        }

        public override INode Analyze(FuncNode astopt)
        {
            Left = Left.Analyze(astopt);
            Right = Right.Analyze(astopt);

            IfIdentiferUsedAs(Left, Right.ExprType);
            IfIdentiferUsedAs(Right, Left.ExprType);

            return this;
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
