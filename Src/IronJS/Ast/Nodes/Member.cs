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
	public class Member : Base {
		public INode Target { get { return Children[0]; } }
		public string Name { get; protected set; }

		public Member(INode target, string name, ITree node)
			: base(NodeType.MemberAccess, node) {
			Children = new[] { target };
			Name = name;
		}

		public override INode Analyze(Stack<Lambda> stack) {
			base.Analyze(stack);
			AnalyzeTools.IfIdentiferUsedAs(Target, IjsTypes.Object);
			return this;
		}
	}
}
