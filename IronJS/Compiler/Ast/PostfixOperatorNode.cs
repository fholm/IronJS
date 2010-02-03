using System;
using System.Linq.Expressions;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Utils;
using IronJS.Runtime2.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
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

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Op);
            Target.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}
