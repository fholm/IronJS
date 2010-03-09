using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {

	public class Vector : Node, INode {
		public Vector(List<INode> values, ITree node)
			: base(NodeType.Array, node) {
			Children = values.ToArray();
		}

		public override Type Type {
			get {
				return IjsTypes.Object;
			}
		}
	}
}
