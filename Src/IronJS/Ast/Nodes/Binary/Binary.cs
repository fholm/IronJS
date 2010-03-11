using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Ast.Tools;
using IronJS.Runtime.Jit;
using IronJS.Runtime.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	using Et = Expression;

	public class Binary : Base {
		public INode Left { get { return Children[0]; } }
		public INode Right { get { return Children[1]; } }
		public ExpressionType Op { get; protected set; }
		public override Type Type { get { return IsComparisonOp ? Types.Boolean : AnalyzeTools.EvalTypes(Left, Right); } }

		public bool IsComparisonOp {
			get {
				return (Op == ExpressionType.LessThan || Op == ExpressionType.LessThanOrEqual
						|| Op == ExpressionType.GreaterThan || Op == ExpressionType.GreaterThanOrEqual
						|| Op == ExpressionType.Equal || Op == ExpressionType.NotEqual);
			}
		}

		public Binary(INode left, INode right, ExpressionType op, ITree node)
			: base(NodeType.BinaryOp, node) {
			Op = op;
			Children = new[] { left, right };
		}

		public override INode Analyze(Stack<Lambda> stack) {
			base.Analyze(stack);

			AnalyzeTools.IfIdentifierAssignedFrom(Left, Right);
			AnalyzeTools.IfIdentifierAssignedFrom(Right, Left);

			return this;
		}

		public override Et Compile(Lambda func) {
			if (AnalyzeTools.TypesAreIdentical(Left, Right)) {
				Et left = Left.Compile(func);
				Et right = Right.Compile(func);

				if (Left.Type == Types.Integer) {
					if (Op == ExpressionType.LessThan)
						return Et.LessThan(left, right);

					if (Op == ExpressionType.Add)
						return Et.Add(left, right);
				}
			}

			throw new NotImplementedException();
		}
	}
}
