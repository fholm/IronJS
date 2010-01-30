using System;
using System.Dynamic;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class DeleteNode : Node
    {
        public INode Target { get; protected set; }

        public DeleteNode(INode target, ITree node)
            : base(NodeType.Delete, node)
        {
            Target = target;
        }

        public override Type ExprType
        {
            get
            {
                return JsTypes.Boolean;
            }
        }

        public override INode Optimize(AstOptimizer astopt)
        {
            Target = Target.Optimize(astopt);

            if (Target is IdentifierNode)
            {
                (Target as IdentifierNode).Variable.UsedAs.Add(JsTypes.Object);
                (Target as IdentifierNode).Variable.CanBeDeleted = true;
            }

            return this;
        }

        public override Et Generate(EtGenerator etgen)
        {
            if (Target is MemberAccessNode)
            {
                var maNode = (MemberAccessNode)Target;

                return EtUtils.Box(
                    Et.Dynamic(
                        etgen.Context.CreateDeleteMemberBinder(maNode.Name),
                        typeof(void),
                        maNode.Target.Generate(etgen)
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
                        iaNode.Target.Generate(etgen),
                        iaNode.Index.Generate(etgen)
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

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);
                Target.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}
