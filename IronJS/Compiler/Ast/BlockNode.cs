using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class BlockNode : Node
    {
        public readonly List<Node> Nodes;
        public readonly bool IsEmpty;

        public BlockNode(List<Node> nodes)
            : base(NodeType.Block)
        {
            Nodes = nodes;
            IsEmpty = nodes.Count == 0;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.Append(indentStr + "(" + Type + "");

            if (!IsEmpty)
            {
                writer.AppendLine();

                foreach (var node in Nodes)
                    node.Print(writer, indent + 1);

                writer.AppendLine(indentStr + ")");
            }
            else
            {
                writer.AppendLine(" empty)");
            }
        }

        public override Et Walk(EtGenerator etgen)
        {
            if (Nodes.Count == 0)
                return Et.Default(typeof(object));

            return Et.Block(
                Nodes.Select(x => x.Walk(etgen))
            );
        }
    }
}
