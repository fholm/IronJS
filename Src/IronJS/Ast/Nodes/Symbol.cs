using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Ast.Tools;

namespace IronJS.Ast.Nodes {
	public class Symbol : Base {
		public string Name { get; protected set; }

		public Symbol(string name, ITree node)
			: base(NodeType.Identifier, node) {
			Name = name;
		}

		public override INode Analyze(Stack<Lambda> stack) {
			return AnalyzeTools.GetVariable(stack, Name);
		}
	}
}
