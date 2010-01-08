using System;
using System.Collections.Generic;
using System.Text;

namespace IronJS.Compiler.Ast
{
    class CallNode : Node
    {
        public readonly Node Target;
        public readonly List<Node> Args;

        public CallNode(Node target, List<Node> args)
            : base(NodeType.Call)
        {
            Target = target;
            Args = args;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);
            Target.Print(writer, indent + 1);

            var argsIndentStr = new String(' ', (indent + 1) * 2);
            writer.AppendLine(argsIndentStr + "(Args");

            foreach (var node in Args)
            {
                node.Print(writer, indent + 2);
            }

            writer.AppendLine(argsIndentStr + ")");
            writer.AppendLine(indentStr + ")");
        }
    }
}
