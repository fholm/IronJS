using System;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	public class Void : Base {
		public INode Target { get { return Children[0]; } }

		public Void(INode target, ITree node)
			: base(NodeType.Void, node) {
			Children = new[] { target };
		}

		public override Type Type {
			get {
				return IjsTypes.Undefined;
			}
		}
	}
}
