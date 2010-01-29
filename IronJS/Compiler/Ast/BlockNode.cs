using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class BlockNode : Node
    {
        public List<Node> Nodes { get; protected set; }
        public bool IsEmpty { get; protected set; }

        public BlockNode(List<Node> nodes, ITree node)
            : base(NodeType.Block, node)
        {
            Nodes = nodes;
            IsEmpty = nodes.Count == 0;
        }

        public override Node Optimize(AstOptimizer astopt)
        {
            var nodes = new List<Node>();

            foreach (var node in Nodes)
                nodes.Add(node.Optimize(astopt));

            Nodes = nodes;
            return this;
        }

        public override Et Generate(EtGenerator etgen)
        {
            if (Nodes.Count == 0)
                return Et.Default(typeof(object));

            return Et.Block(
                Nodes.Select(x => x.Generate(etgen))
            );
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

    }
}
