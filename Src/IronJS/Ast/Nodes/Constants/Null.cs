using System;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	public class Null : Base, INode {
		public Null(ITree node)
			: base(NodeType.Null, node) {

		}

		public override Type Type {
			get {
				return Types.Dynamic;
			}
		}
	}
}
