using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
using System.Text;

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
        public override Type ExprType { get { return IsComparisonOp ? IjsTypes.Boolean : EvalTypes(Left, Right); } }

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

        public override INode Analyze(Function func)
        {
            Left = Left.Analyze(func);
            Right = Right.Analyze(func);

            IfIdentifierAssignedFrom(Left, Right);
            IfIdentifierAssignedFrom(Right, Left);

            return this;
        }

        public override Et Compile(Function func)
        {
            if (IdenticalTypes(Left, Right))
            {
                Et left = Left.Compile(func);
                Et right = Right.Compile(func);

                if (Left.ExprType == IjsTypes.Integer)
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

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Op);

            Left.Print(writer, indent + 1);
            Right.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
