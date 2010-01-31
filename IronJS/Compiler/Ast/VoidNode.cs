using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class VoidNode : Node
    {
        public INode Target { get; protected set; }

        public VoidNode(INode target, ITree node)
            : base(NodeType.Void, node)
        {
            Target = target;
        }

        public override Type ExprType
        {
            get
            {
                return IjsTypes.Undefined;
            }
        }

        public override Et Generate(EtGenerator etgen)
        {
            return Et.Block(
                Target.Generate(etgen),
                Undefined.Expr
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            if (Target != null)
                Target.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
