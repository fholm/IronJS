using System.Collections.Generic;
using Antlr.Runtime.Tree;

namespace IronJS.Ast.Nodes {
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

				function.CreateVar(symbol.Name, new Local(symbol.Name));
			}

			return base.Analyze(stack);
		}
	}
}
