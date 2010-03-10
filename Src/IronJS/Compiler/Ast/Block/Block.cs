using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
	using Et = Expression;

	public class Block : Node {
		public Block(List<INode> nodes, ITree node)
			: base(NodeType.Block, node) {
			Children = nodes.ToArray();
		}

		public override Et Compile(Lambda func) {
			return AstTools.BuildBlock(Children, delegate(INode node) {
				return node.Compile(func);
			});
		}
	}
}
