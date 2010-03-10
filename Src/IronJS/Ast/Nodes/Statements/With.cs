using Antlr.Runtime.Tree;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	public class With : Base {
		public INode Target { get { return Children[0]; } }
		public INode Body { get { return Children[1]; } }

		public With(INode target, INode body, ITree node)
			: base(NodeType.With, node) {
			Children = new[] { target, body };
		}
	}
}
