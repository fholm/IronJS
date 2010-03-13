using System;
using System.Collections.Generic;
using IronJS.Ast.Tools;
using Antlr.Runtime.Tree;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	using Et = Expression;

	public class Var : Base {
		public INode Target { get { return Children[0]; } }

		public Var(INode target, ITree node)
			: base(NodeType.Var, node) {
			Children = new[] { target };
		}

		public override INode Analyze(Stack<Lambda> stack) {
			Lambda function = stack.Peek();

			if (stack.Count > 1) {
				Symbol symbol = Target as Symbol;

				if (symbol == null) {
					Assign assign = Target as Assign;

					if (assign == null) {
						throw new AstError("Var must have Assign or Symbol child");
					}

					symbol = assign.Target as Symbol;
				}

                function.Vars.Add(Node.Variable(symbol.Name));
			}

			return base.Analyze(stack);
		}

		public override Et Compile(Lambda func) {
			return Target.Compile(func);
		}
	}
}
