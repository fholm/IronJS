using System;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class CatchNode : Node
    {
        public INode Target { get; protected set; }
        public INode Body { get; protected set; }

        public CatchNode(INode target, INode body, ITree node)
            : base(NodeType.Catch, node)
        {
            Target = target;
            Body = body;
        }

        public override Et Generate(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }

        public override void Print(System.Text.StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type + "");

            if (Target != null)
                Target.Print(writer, indent + 1);

            Body.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}
