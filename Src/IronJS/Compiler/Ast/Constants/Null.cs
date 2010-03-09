using System;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
	public class Null : Node, INode {
		public Null(ITree node)
			: base(NodeType.Null, node) {

		}

		public override Type Type {
			get {
				return IjsTypes.Dynamic;
			}
		}
	}
}
