using Antlr.Runtime.Tree;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
	public class Return : Node {
		public INode Value { get { return Children[0]; } }
		public Lambda FuncNode { get; protected set; }

		public Return(INode value, ITree node)
			: base(NodeType.Return, node) {
			Children = new[] { value };
		}
	}
}
