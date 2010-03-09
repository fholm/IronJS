using Antlr.Runtime.Tree;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
	public class For : Node {
		public INode Setup { get { return Children[0]; } }
		public INode Test { get { return Children[1]; } }
		public INode Incr { get { return Children[2]; } }
		public INode Body { get { return Children[3]; } }

		public For(INode setup, INode test, INode incr, INode body, ITree node)
			: base(NodeType.ForStep, node) {
			Children = new[] { setup, test, incr, body };
		}
	}
}
