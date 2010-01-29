using System;
using System.Linq.Expressions;
using System.Text;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class LogicalNode : Node
    {
        public Node Left { get; protected set; }
        public Node Right { get; protected set; }
        public ExpressionType Op { get; protected set; }

        public LogicalNode(Node left, Node right, ExpressionType op)
            : base(NodeType.Logical)
        {
            Left = left;
            Right = right;
            Op = op;
        }


        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Op);

            Left.Print(writer, indent + 1);
            Right.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }

        public override Expression Walk(EtGenerator etgen)
        {
            var tmp = Et.Parameter(typeof(object), "#tmp");

            return Et.Block(
                new[] { tmp },

                Et.Assign(
                    tmp,
                    EtUtils.Cast<object>(Left.Walk(etgen))
                ),

                Et.Condition(
                    Et.Dynamic(
                        etgen.Context.CreateConvertBinder(typeof(bool)),
                        typeof(bool),
                        tmp
                    ),

                    EtUtils.Cast<object>(
                        Op == ExpressionType.AndAlso
                               ? Right.Walk(etgen) // &&
                               : tmp               // ||
                    ),

                    EtUtils.Cast<object>(
                        Op == ExpressionType.AndAlso
                               ? tmp               // &&
                               : Right.Walk(etgen) // ||
                    )
                )
            );
        }
    }
}
