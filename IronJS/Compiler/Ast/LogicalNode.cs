using System;
using System.Linq.Expressions;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class LogicalNode : Node
    {
        public INode Left { get; protected set; }
        public INode Right { get; protected set; }
        public ExpressionType Op { get; protected set; }

        public LogicalNode(INode left, INode right, ExpressionType op, ITree node)
            : base(NodeType.Logical, node)
        {
            Left = left;
            Right = right;
            Op = op;
        }

        public override Type ExprType
        {
            get
            {
                if (Left.ExprType == Right.ExprType)
                    return Left.ExprType;

                return IjsTypes.Dynamic;
            }
        }


        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Op);

            Left.Print(writer, indent + 1);
            Right.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }

        public override Expression Generate(EtGenerator etgen)
        {
            var tmp = Et.Parameter(typeof(object), "#tmp");

            return Et.Block(
                new[] { tmp },

                Et.Assign(
                    tmp,
                    EtUtils.Cast<object>(Left.Generate(etgen))
                ),

                Et.Condition(
                    Et.Dynamic(
                        etgen.Context.CreateConvertBinder(typeof(bool)),
                        typeof(bool),
                        tmp
                    ),

                    EtUtils.Cast<object>(
                        Op == ExpressionType.AndAlso
                               ? Right.Generate(etgen) // &&
                               : tmp               // ||
                    ),

                    EtUtils.Cast<object>(
                        Op == ExpressionType.AndAlso
                               ? tmp               // &&
                               : Right.Generate(etgen) // ||
                    )
                )
            );
        }
    }
}
