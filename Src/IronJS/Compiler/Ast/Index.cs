using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using System.Collections.Generic;
using IronJS.Compiler.Tools;

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

		public override INode Analyze(Stack<Function> stack) {
			base.Analyze(stack);

			AnalyzeTools.IfIdentiferUsedAs(Target, IjsTypes.Object);

			return this;
		}
	}
}
