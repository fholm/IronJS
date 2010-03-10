using System;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	//TODO: Add UsedAs type to all unary operators
	public class Unary : Base {
		public INode Target { get { return Children[0]; } }
		public ExpressionType Op { get; protected set; }

		public Unary(INode target, ExpressionType op, ITree node)
			: base(NodeType.UnaryOp, node) {
			Children = new[] { target };
			Op = op;
		}

		public override Type Type {
			get {
				if (Op == ExpressionType.Not)
					return Types.Boolean;

				if (Op == ExpressionType.OnesComplement)
					return Types.Integer;

				if (Op == ExpressionType.UnaryPlus)
					return Types.Double;

				if (Op == ExpressionType.Negate)
					return Types.Double;

				throw new AstError("Unrecognized unary operator '{0}'", Op);
			}
		}
	}
}
