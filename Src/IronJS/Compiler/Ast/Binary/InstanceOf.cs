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
	public class InstanceOf : Node {
		public INode Target { get { return Children[0]; } }
		public INode Function { get { return Children[1]; } }

		public InstanceOf(INode target, INode function, ITree node)
			: base(NodeType.InstanceOf, node) {
			Children = new[] { target, function };
		}

		public override Type Type {
			get {
				return IjsTypes.Boolean;
			}
		}

		public override INode Analyze(Stack<Lambda> stack) {
			base.Analyze(stack);

			AnalyzeTools.IfIdentiferUsedAs(Target, IjsTypes.Object);
			AnalyzeTools.IfIdentiferUsedAs(Function, IjsTypes.Object);

			return this;
		}
	}
}
