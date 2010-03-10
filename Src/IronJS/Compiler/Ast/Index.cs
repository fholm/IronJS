using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Tools;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
	public class Index : Node {
		public INode Target { get { return Children[0]; } }
		public INode Value { get { return Children[1]; } }

		public Index(INode target, INode index, ITree node)
			: base(NodeType.IndexAccess, node) {
			Children = new[] { target, index };
		}

		public override INode Analyze(Stack<Lambda> stack) {
			base.Analyze(stack);
			AnalyzeTools.IfIdentiferUsedAs(Target, IjsTypes.Object);
			return this;
		}
	}
}
