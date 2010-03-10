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
	public class UnsignedRShift : Base {
		public INode Left { get { return Children[0]; } }
		public INode Right { get { return Children[1]; } }

		public UnsignedRShift(INode left, INode right, ITree node)
			: base(NodeType.UnsignedRShift, node) {
			Children = new[] { left, right };
		}

		public override Type Type {
			get {
				return IjsTypes.Integer;
			}
		}

		public override INode Analyze(Stack<Lambda> stack) {
			base.Analyze(stack);

			AnalyzeTools.IfIdentifierAssignedFrom(Left, Right);
			AnalyzeTools.IfIdentifierAssignedFrom(Right, Left);

			return this;
		}
	}
}
