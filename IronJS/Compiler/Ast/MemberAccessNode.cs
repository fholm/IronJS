using System;
using System.Text;

namespace IronJS.Compiler.Ast
{
    using Et = System.Linq.Expressions.Expression;

    class MemberAccessNode : Node
    {
        public readonly Node Target;
        public readonly string Name;

        public MemberAccessNode(Node target, string member)
            : base(NodeType.MemberAccess)
        {
            Target = target;
            Name = member;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type + " " + Name);
            Target.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }

        public override Et Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
