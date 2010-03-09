using Antlr.Runtime.Tree;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
	public enum WhileType { DoWhile, While }

	public class While : Node {
		public INode Test { get { return Children[0]; } }
		public INode Body { get { return Children[1]; } }
		public WhileType LoopType { get; protected set; }

		public While(INode test, INode body, WhileType type, ITree node)
			: base(NodeType.While, node) {
			Children = new[] { test, body };
			LoopType = type;
		}
	}
}
