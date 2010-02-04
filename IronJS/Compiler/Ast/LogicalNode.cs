using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;

namespace IronJS.Compiler.Ast
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;
    using System.Text;

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

        public override void Print(StringBuilder writer, int indent)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Op);

            Left.Print(writer, indent + 1);
            Right.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
