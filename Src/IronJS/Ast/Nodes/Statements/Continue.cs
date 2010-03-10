using Antlr.Runtime.Tree;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	public class Continue : Base, INode {
		public string Label { get; protected set; }

		public Continue(string label, ITree node)
			: base(NodeType.Continue, node) {
			Label = label;
		}
	}
}
