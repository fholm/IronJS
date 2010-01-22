using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;
using Microsoft.Scripting.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class CallNode : Node
    {
        public readonly Node Target;
        public readonly List<Node> Args;

        public CallNode(Node target, List<Node> args)
            : base(NodeType.Call)
        {
            Target = target;
            Args = args;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);
            Target.Print(writer, indent + 1);

            var argsIndentStr = new String(' ', (indent + 1) * 2);
            writer.AppendLine(argsIndentStr + "(Args");

            foreach (var node in Args)
                node.Print(writer, indent + 2);

            writer.AppendLine(argsIndentStr + ")");
            writer.AppendLine(indentStr + ")");
        }

        public override Et Walk(EtGenerator etgen)
        {
            var args = Args.Select(x => x.Walk(etgen)).ToArray();

            if (Target is MemberAccessNode)
            {
                var target = (Ast.MemberAccessNode)Target;
                var tmp = Et.Variable(typeof(object), "#tmp");
                var targetExpr = etgen.GenerateConvertToObject(
                        target.Target.Walk(etgen)
                    );

                return Et.Block(
                    new[] { tmp },
                    Et.Assign(
                        tmp,
                        targetExpr
                    ),
                    Et.Dynamic(
                        etgen.Context.CreateInvokeMemberBinder(
                            target.Name,
                            new CallInfo(args.Length + 1)
                        ),
                        typeof(object),
                        ArrayUtils.Insert(
                            tmp,
                            tmp,
                            args
                        )
                    )
                );
            }

            return Et.Dynamic(
                etgen.Context.CreateInvokeBinder(
                    new CallInfo(args.Length)
                ),
                typeof(object),
                ArrayUtils.Insert(
                    Target.Walk(etgen),
                    Scope.EtValue(
                        etgen.GlobalScopeExpr
                    ),
                    args
                )
            );
        }
    }
}
