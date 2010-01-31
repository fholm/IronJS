using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class BlockNode : Node
    {
        public List<INode> Nodes { get; protected set; }
        public bool IsEmpty { get { return Nodes == null || Nodes.Count == 0; } }

        public BlockNode(List<INode> nodes, ITree node)
            : base(NodeType.Block, node)
        {
            Nodes = nodes;
        }

        public override INode Analyze(IjsAstAnalyzer astopt)
        {
            var nodes = new List<INode>();

            foreach (var node in Nodes)
                nodes.Add(node.Analyze(astopt));

            Nodes = nodes;
            return this;
        }

        public override Et EtGen(IjsEtGenerator etgen)
        {
            if (IsEmpty)
                return AstUtils.Empty();

            return Et.Block(
                Nodes.Select(x => x.EtGen(etgen))
            );
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

            writer.Append(indentStr + "(" + NodeType + "");

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
