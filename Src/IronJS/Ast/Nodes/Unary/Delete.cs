using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Binders;
using IronJS.Runtime.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	using AstUtils = Microsoft.Scripting.Ast.Utils;
	using Et = Expression;

	public class Delete : Base {
		public INode Target { get { return Children[0]; } }

		public Delete(INode target, ITree node)
			: base(NodeType.Delete, node) {
			Children = new[] { target };
		}

		public override Type Type {
			get {
				return Types.Boolean;
			}
		}
	}
}
