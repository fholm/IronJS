using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	public class List : Base, INode {
		public List(List<INode> values, ITree node)
			: base(NodeType.Array, node) {
			Children = values.ToArray();
		}

		public override Type Type {
			get {
				return Types.Object;
			}
		}
	}
}
