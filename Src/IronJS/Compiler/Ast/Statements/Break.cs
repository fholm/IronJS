using Antlr.Runtime.Tree;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
	public class Break : Node {
		public string Label { get; protected set; }

		public Break(string label, ITree node)
			: base(NodeType.Break, node) {
			Label = label;
		}
	}
}
