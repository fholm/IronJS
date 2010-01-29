using System;
using Antlr.Runtime.Tree;

namespace IronJS.Compiler.Ast
{
    public class CatchNode : Node
    {
        public Node Target { get; protected set; }
        public Node Body { get; protected set; }

        public CatchNode(Node target, Node body, ITree node)
            : base(NodeType.Catch, node)
        {
            Target = target;
            Body = body;
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

        public override System.Linq.Expressions.Expression Generate(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
