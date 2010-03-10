using System;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	public class Postfix : Base {
		public INode Target { get { return Children[0]; } }
		public ExpressionType Op { get; protected set; }

		public Postfix(INode target, ExpressionType op, ITree tree)
			: base(NodeType.PostfixOperator, tree) {
			Children = new[] { target };
			Op = op;
		}

		public override Type Type {
			get {
				if (Target.Type == IjsTypes.Integer)
					return IjsTypes.Integer;

				return IjsTypes.Double;
			}
		}
	}
}
