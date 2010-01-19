using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Scripting.Utils;
using IronJS.Runtime.Js;

using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Restrict = System.Dynamic.BindingRestrictions;
using EtParam = System.Linq.Expressions.ParameterExpression;

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
