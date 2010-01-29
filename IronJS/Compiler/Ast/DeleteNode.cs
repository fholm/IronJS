using System;
using System.Dynamic;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class DeleteNode : Node
    {
        public Node Target { get; protected set; }

        public DeleteNode(Node target)
            : base(NodeType.Delete)
        {
            Target = target;
        }

        public override Et Walk(EtGenerator etgen)
        {
            if (Target is MemberAccessNode)
            {
                var maNode = (MemberAccessNode)Target;

                return EtUtils.Box(
                    Et.Dynamic(
                        etgen.Context.CreateDeleteMemberBinder(maNode.Name),
                        typeof(void),
                        maNode.Target.Walk(etgen)
                    )
                );
            }

            if (Target is IndexAccessNode)
            {
                var iaNode = (IndexAccessNode)Target;

                return EtUtils.Box(
                    Et.Dynamic(
                        etgen.Context.CreateDeleteIndexBinder(new CallInfo(1)),
                        typeof(void),
                        iaNode.Target.Walk(etgen),
                        iaNode.Index.Walk(etgen)
                    )
                );
            }

            if (Target is IdentifierNode)
            {
                var idNode = (IdentifierNode)Target;

                return Et.Call(
                    etgen.FunctionScope.ScopeExpr,
                    Scope.MiDelete,
                    Et.Constant(idNode.Name, typeof(object))
                );
            }

            throw new NotImplementedException();
        }

        public override void Print(System.Text.StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);
                Target.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}
