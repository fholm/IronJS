using System;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	public class Logical : Base {
		public INode Left { get { return Children[0]; } }
		public INode Right { get { return Children[1]; } }
		public ExpressionType Op { get; protected set; }

		public Logical(INode left, INode right, ExpressionType op, ITree node)
			: base(NodeType.Logical, node) {
			Op = op;
			Children = new[] { left, right };
		}

		public override Type Type {
			get {
				if (Left.Type == Right.Type)
					return Left.Type;

				return Types.Dynamic;
			}
		}
	}
}
