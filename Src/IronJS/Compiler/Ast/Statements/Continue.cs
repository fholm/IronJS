using Antlr.Runtime.Tree;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
	public class Continue : Node, INode {
		public string Label { get; protected set; }

		public Continue(string label, ITree node)
			: base(NodeType.Continue, node) {
			Label = label;
		}
	}
}
