using System;
using System.Text;
using Antlr.Runtime.Tree;

namespace IronJS.Ast.Nodes {
	public class Regex : Base {
		public string RegexStr { get; protected set; }
		public string Modifiers { get; protected set; }

		public Regex(string regex, ITree node)
			: base(NodeType.Regex, node) {
			int lastIndex = regex.LastIndexOf('/');
			RegexStr = regex.Substring(1, lastIndex - 1);
			Modifiers = regex.Substring(lastIndex + 1);
		}
	}
}
