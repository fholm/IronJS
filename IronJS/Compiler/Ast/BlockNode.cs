using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;

namespace IronJS.Compiler.Ast
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;
    using System.Text;

    public class BlockNode : Node
    {
        public List<INode> Nodes { get; protected set; }
        public bool IsEmpty { get { return Nodes == null || Nodes.Count == 0; } }

        public BlockNode(List<INode> nodes, ITree node)
            : base(NodeType.Block, node)
        {
            Nodes = nodes;
        }

        public override INode Analyze(FuncNode astopt)
        {
            var nodes = new List<INode>();

            foreach (var node in Nodes)
                nodes.Add(node.Analyze(astopt));

            Nodes = nodes;
            return this;
        }

        public override Et EtGen(FuncNode func)
        {
            return AstTools.BuildBlock(Nodes, delegate(INode node){
                return node.EtGen(func);
            });
        }

        public override void Print(StringBuilder writer, int indent)
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
