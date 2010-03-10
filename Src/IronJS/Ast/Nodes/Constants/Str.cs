using System;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	using Et = Expression;

	public class Str : Base, INode {
		public string Value { get; protected set; }
		public char Delimiter { get; protected set; }

		public Str(string value, char delimiter, ITree node)
			: base(NodeType.String, node) {
			Value = value;
			Delimiter = delimiter;
		}

		public override Type Type {
			get {
				return Types.String;
			}
		}

		public override Et Compile(Lambda etgen) {
			return AstTools.Constant(Value);
		}

		public override string ToString() {
			return base.ToString() + " " + Value.ToString();
		}
	}
}
