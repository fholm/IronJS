using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Ast.Tools;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	public class ForIn : Base {
		public INode Target { get { return Children[0]; } }
		public INode Source { get { return Children[1]; } }
		public INode Body { get { return Children[2]; } }

		public ForIn(INode target, INode source, INode body, ITree node)
			: base(NodeType.ForIn, node) {
			Children = new[] { target, source, body };
		}

		public override INode Analyze(Stack<Lambda> stack) {
			base.Analyze(stack);
			AnalyzeTools.IfIdentiferUsedAs(Source, IjsTypes.Object);
			return this;
		}
	}
}
