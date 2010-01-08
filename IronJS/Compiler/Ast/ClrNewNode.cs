using System;
using System.Collections.Generic;
using System.Text;

namespace IronJS.Compiler.Ast
{
    class ClrNewNode : Node
    {
        public readonly Node TypeName;
        public readonly List<Node> Args;

        public ClrNewNode(Node typeName, List<Node> args)
            : base(NodeType.ClrNew)
        {
            this.TypeName = typeName;
            this.Args = args;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);
            TypeName.Print(writer, indent + 1);

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
