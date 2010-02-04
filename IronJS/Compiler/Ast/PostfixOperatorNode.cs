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

    public class PostfixOperatorNode : Node
    {
        public INode Target { get; protected set; }
        public ExpressionType Op { get; protected set; }

        public PostfixOperatorNode(INode node, ExpressionType op, ITree tree)
            : base(NodeType.PostfixOperator, tree)
        {
            Target = node;
            Op = op;
        }

        public override Type ExprType
        {
            get
            {
                if (Target.ExprType == IjsTypes.Integer)
                    return IjsTypes.Integer;

                return IjsTypes.Double;
            }
        }

        public override INode Analyze(FuncNode astopt)
        {
            Target = Target.Analyze(astopt);
            return this;
        }

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Op);
            Target.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}
