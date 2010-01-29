using System;
using System.Text;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class InNode : Node
    {
        public Node Target { get; protected set; }
        public Node Property { get; protected set; }

        public InNode(Node target, Node property)
            : base(NodeType.In)
        {
            Target = target;
            Property = property;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Call(
                EtUtils.Cast<IObj>(Target.Walk(etgen)),
                IObjMethods.MiHasProperty,
                Property.Walk(etgen)
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);

            Property.Print(writer, indent + 1);
            Target.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
