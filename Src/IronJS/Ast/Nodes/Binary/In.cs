using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Ast.Tools;
using IronJS.Runtime.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {

	public class In : Base {
		public INode Target { get { return Children[0]; } }
		public INode Property { get { return Children[1]; } }

		public In(INode target, INode property, ITree node)
			: base(NodeType.In, node) {
			Children = new[] { target, property };
		}

		public override Type Type {
			get {
				return Types.Boolean;
			}
		}

		public override INode Analyze(Stack<Lambda> stack) {
			base.Analyze(stack);
			AnalyzeTools.IfIdentiferUsedAs(Target, Types.Object);
			return this;
		}
	}
}
