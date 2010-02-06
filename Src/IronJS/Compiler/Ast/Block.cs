using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {

    #region Aliases
    using Et = Expression;
    #endregion

    public class Block : Node {
        public List<INode> Nodes { get; protected set; }

        public Block(List<INode> nodes, ITree node)
            : base(NodeType.Block, node) {
            Nodes = nodes;
        }

        public override INode Analyze(Stack<Function> stack) {
            for (int index = 0; index < Nodes.Count; ++index) {
                Nodes[index] = Nodes[index].Analyze(stack);
            }

            return this;
        }

        public override Et Compile(Function func) {
            return AstTools.BuildBlock(Nodes, delegate(INode node) {
                return node.Compile(func);
            });
        }

        public override void Write(StringBuilder writer, int indent) {
            string indentStr = StringTools.Repeat(" ", indent * 2);

            writer.Append(indentStr + "(" + NodeType + "");
            writer.AppendLine();

            foreach (INode node in Nodes)
                node.Write(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }

    }
}
