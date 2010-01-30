using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class MemberAccessNode : Node
    {
        public INode Target { get; protected set; }
        public string Name { get; protected set; }

        public MemberAccessNode(INode target, string member, ITree node)
            : base(NodeType.MemberAccess, node)
        {
            Target = target;
            Name = member;
        }

        public override INode Analyze(AstAnalyzer astopt)
        {
            //if (Target is IdentifierNode)
            //    (Target as IdentifierNode).Variable.AssignedFrom.Add(GetType());

            return this;
        }

        public override Et Generate(EtGenerator etgen)
        {
            return Et.Dynamic(
                etgen.Context.CreateGetMemberBinder(Name),
                typeof(object),
                Et.Dynamic(
                    etgen.Context.CreateConvertBinder(typeof(IObj)),
                    typeof(object),
                    Target.Generate(etgen)
                )
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType + " " + Name);
            Target.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}
