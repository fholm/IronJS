using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Ast.Tools;
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

	public class Bool : Base {
		public bool Value { get; protected set; }

		public Bool(bool value, ITree node)
			: base(NodeType.Boolean, node) {
			Value = value;
		}

		public override Type Type {
			get {
				return IjsTypes.Boolean;
			}
		}

		public override Et Compile(Lambda func) {
			return AstTools.Constant(Value);
		}

		public override string ToString() {
			return base.ToString() + " " + Value.ToString();
		}
	}
}
