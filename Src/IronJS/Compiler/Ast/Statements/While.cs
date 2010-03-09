using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
	using AstUtils = Microsoft.Scripting.Ast.Utils;

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
