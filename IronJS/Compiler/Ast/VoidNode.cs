using System;
using System.Text;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{ 
    public class VoidNode : Node
    {
        public Node Target { get; protected set; }

        public VoidNode(Node target)
            : base(NodeType.Void)
        {
            Target = target;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Block(
                Target.Walk(etgen),
                Undefined.Expr
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);

            if (Target != null)
                Target.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
