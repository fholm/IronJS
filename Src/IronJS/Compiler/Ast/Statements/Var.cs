using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Tools;

namespace IronJS.Compiler.Ast {
	public class Var : Node {
		public INode Target { get { return Children[0]; } }

		public Var(INode target, ITree node)
			: base(NodeType.Var, node) {
			Children = new[] { target };
		}

		public override INode Analyze(Stack<Function> stack) {
			Function function = stack.Peek();

			if (stack.Count > 1) {
				Symbol symbol = Target as Symbol;

				if (symbol == null) {
					Assign assign = Target as Assign;

					if (assign == null) {
						throw new AstCompilerError("Var must have Assign or Symbol child");
					}

					symbol = assign.Target as Symbol;
				}

				function.Var(symbol.Name, new Local(symbol.Name));
			}

			return base.Analyze(stack);
		}
	}
}
