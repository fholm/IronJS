using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Tools;

namespace IronJS.Compiler.Ast {
	public class Symbol : Node {
		public string Name { get; protected set; }

		public Symbol(string name, ITree node)
			: base(NodeType.Identifier, node) {
			Name = name;
		}

		public override INode Analyze(Stack<Function> stack) {
			return AnalyzeTools.GetVariable(stack, Name);
		}
	}
}
