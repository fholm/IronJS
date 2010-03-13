using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using IronJS.Runtime.Jit;
using IronJS.Ast.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	using AstUtils = Utils;
	using Et = Expression;

	public enum NodeType {
		Assign, Identifier, Double, Null,
		MemberAccess, Call, If, Eq, Block,
		String, Lambda, While, BinaryOp,
		Object, New, AutoProperty, Return,
		UnaryOp, Logical, PostfixOperator,
		TypeOf, Boolean, Void, StrictCompare,
		UnsignedRShift, ForStep, ForIn,
		Break, Continue, With, Try, Catch,
		Throw, IndexAccess, Delete, In,
		Switch, InstanceOf, Regex, Array,
		Integer, Var, Param, Local,
		Global, Closed
	}

	abstract public class Base : INode {
		public string Debug { get { return DisplayTools.AsString(this); } }
		public NodeType NodeType { get; protected set; }
		public INode[] Children { get; protected set; }
		public virtual Type Type { get { return Types.Dynamic; } }

		public Base(NodeType type, ITree node) {
			NodeType = type;
		}

        public Base(NodeType type)
            : this(type, null) {
        }

		public virtual INode Analyze(Stack<Lambda> stack) {
			if (Children != null) {
				for (int i = 0; i < Children.Length; ++i) {
					if (Children[i] != null) {
						Children[i] = Children[i].Analyze(stack);
					}
				}
			}

			return this;
		}

		public virtual Et Compile(Lambda func) {
			return AstUtils.Default(typeof(object));
		}

		public override string ToString() {
			return NodeType.ToString();
		}
	}
}
