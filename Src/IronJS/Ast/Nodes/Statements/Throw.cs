using Antlr.Runtime.Tree;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	public class Throw : Base {
		public INode Value { get { return Children[0]; } }

		public Throw(INode value, ITree node)
			: base(NodeType.Throw, node) {
			Children = new[] { Value };
		}
	}
}
