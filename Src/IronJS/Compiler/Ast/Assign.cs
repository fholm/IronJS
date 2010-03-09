using System;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Tools;
using System.Collections.Generic;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
	using Et = Expression;

	public class Assign : Node, INode {
		public INode Target { get { return Children[0]; } }
		public INode Value { get { return Children[1]; } }

		public override Type Type { get { return Value.Type; } }

		public Assign(INode target, INode value, ITree node)
			: base(NodeType.Assign, node) {
			Children = new[] { target, value };
		}


		public override Et Compile(Function func) {
			return CompileTools.Assign(func, Target, Value.Compile(func));
		}

		public override INode Analyze(Stack<Function> stack) {
			base.Analyze(stack);

			Closed closed = Target as Closed;
			if (closed != null)
				AnalyzeTools.AddClosedType(stack, closed.Name, Value.Type);

			AnalyzeTools.IfIdentifierAssignedFrom(Target, Value);

			return this;
		}
	}
}
