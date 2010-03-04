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

namespace IronJS.Compiler.Ast
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;

    public class Binary : Node
    {
        public INode Left { get; protected set; }
        public INode Right { get; protected set; }
        public ExpressionType Op { get; protected set; }
        public override Type Type { get { return IsComparisonOp ? IjsTypes.Boolean : AnalyzeTools.EvalTypes(Left, Right); } }

        public bool IsComparisonOp
        {
            get
            {
                return (   Op == ExpressionType.LessThan    || Op == ExpressionType.LessThanOrEqual
                        || Op == ExpressionType.GreaterThan || Op == ExpressionType.GreaterThanOrEqual
                        || Op == ExpressionType.Equal       || Op == ExpressionType.NotEqual);
            }
        }

        public Binary(INode left, INode right, ExpressionType op, ITree node)
            : base(NodeType.BinaryOp, node)
        {
            Op = op;
            Left = left;
            Right = right;
        }

        public override INode Analyze(Stack<Function> func)
        {
            Left = Left.Analyze(func);
            Right = Right.Analyze(func);

			AnalyzeTools.IfIdentifierAssignedFrom(Left, Right);
			AnalyzeTools.IfIdentifierAssignedFrom(Right, Left);

            return this;
        }

        public override Et Compile(Function func)
        {
			if (AnalyzeTools.IdenticalTypes(Left, Right))
            {
                Et left = Left.Compile(func);
                Et right = Right.Compile(func);

                if (Left.Type == IjsTypes.Integer)
                {
                    if (Op == ExpressionType.LessThan)
                        return Et.LessThan(left, right);

                    if (Op == ExpressionType.Add)
                        return Et.Add(left, right);
                }
            }
            else
            {

            }

            throw new NotImplementedException();
        }
    }
}
