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

	public class Try : Base {
		public INode Body { get { return Children[0]; } }
		public INode Target { get { return Children[1]; } }
		public INode Catch { get { return Children[2]; } }
		public INode Finally { get { return Children[3]; } }

		public Try(INode body, INode target, INode _catch, INode _finally, ITree node)
			: base(NodeType.Try, node) {
			Children = new[] { body, target, _catch, _finally };
		}

		public override INode Analyze(Stack<Lambda> stack) {
			base.Analyze(stack);
			AnalyzeTools.IfIdentiferUsedAs(Target, IjsTypes.Object);
			return this;
		}
	}
}
