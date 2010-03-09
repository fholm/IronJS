using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
using System.Text;
using IronJS.Compiler.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
	using AstUtils = Microsoft.Scripting.Ast.Utils;
	using Et = Expression;

	public class ForIn : Node {
		public INode Target { get { return Children[0]; } }
		public INode Source { get { return Children[1]; } }
		public INode Body { get { return Children[2]; } }

		public ForIn(INode target, INode source, INode body, ITree node)
			: base(NodeType.ForIn, node) {
			Children = new[] { target, source, body };
		}

		public override INode Analyze(Stack<Function> stack) {
			base.Analyze(stack);

			AnalyzeTools.IfIdentiferUsedAs(Source, IjsTypes.Object);

			return this;
		}
	}
}
