using System;
using System.Dynamic;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;
using IronJS.Runtime2.Js;

namespace IronJS.Compiler.Ast
{
    public class DeleteNode : Node
    {
        public INode Target { get; protected set; }

        public DeleteNode(INode target, ITree node)
            : base(NodeType.Delete, node)
        {
            Target = target;
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
            Target = Target.Analyze(astopt);
            return this;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);
                Target.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}
