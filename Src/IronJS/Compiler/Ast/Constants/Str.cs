using System;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
	using Et = Expression;

	public class Str : Node, INode {
		public string Value { get; protected set; }
		public char Delimiter { get; protected set; }

		public Str(string value, char delimiter, ITree node)
			: base(NodeType.String, node) {
			Value = value;
			Delimiter = delimiter;
		}

		public override Type Type {
			get {
				return IjsTypes.String;
			}
		}

		public override Et Compile(Function etgen) {
			return AstTools.Constant(Value);
		}

		public override string ToString() {
			return base.ToString() + " " + Value.ToString();
		}
	}
}
