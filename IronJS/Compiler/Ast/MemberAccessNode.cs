using System;
using System.Text;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
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
            return Et.Dynamic(
                etgen.Context.CreateGetMemberBinder(Name),
                typeof(object),
                Et.Dynamic(
                    etgen.Context.CreateConvertBinder(typeof(IObj)),
                    typeof(object),
                    Target.Walk(etgen)
                )
            );
        }
    }
}
