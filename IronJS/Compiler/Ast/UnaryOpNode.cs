using System;
using System.Linq.Expressions;
using System.Text;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class UnaryOpNode : Node
    {
        public Node Target { get; protected set; }
        public ExpressionType Op { get; protected set; }

        public UnaryOpNode(Node target, ExpressionType op)
            : base(NodeType.UnaryOp)
        {
            Target = target;
            Op = op;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Dynamic(
                etgen.Context.CreateUnaryOpBinder(Op),
                typeof(object),
                Target.Walk(etgen)
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Op);

            if (Target != null)
                Target.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
