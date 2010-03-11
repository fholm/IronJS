using System;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using IronJS.Tools;
using IronJS.Runtime.Jit;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	using Et = Expression;

	public class NumberNode<T> : Base, INode {
		public T Value { get; protected set; }

		public NumberNode(T value, NodeType type, ITree node)
			: base(type, node) {
			Value = value;
		}

		public override Type Type {
			get {
				if (this.GetType() == typeof(NumberNode<long>))
					return Types.Integer;

				return Types.Double;
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
