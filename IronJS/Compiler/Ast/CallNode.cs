using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class CallNode : Node
    {
        public INode Target { get; protected set; }
        public List<INode> Args { get; protected set; }

        public CallNode(INode target, List<INode> args, ITree node)
            : base(NodeType.Call, node)
        {
            Target = target;
            Args = args;
        }

        public override INode Optimize(AstOptimizer astopt)
        {
            Target = Target.Optimize(astopt);

            var args = new List<INode>();
            foreach (var arg in Args)
                args.Add(arg.Optimize(astopt));
            Args = args;

            return this;
        }

        public override Et Generate(EtGenerator etgen)
        {
            var args = Args.Select(
                x => EtUtils.Cast<object>(x.Generate(etgen))
            ).ToArray();

            /*
             * This is a fix for calls inside with-statements
             * that look like normal function calls, i.e.
             * 
             * with(foo) { 
             *     bar(); 
             * }
             * 
             * But 'bar' can here be either a function existing
             * in the global scope, or a method on foo.
             * */
            if (Target is IdentifierNode && etgen.IsInsideWith)
            {
                return Et.Call(
                    etgen.FunctionScope.ScopeExpr,
                    typeof(Scope).GetMethod("Call"),
                    Et.Constant((Target as IdentifierNode).Name),
                    Et.NewArrayInit(
                        typeof(object),
                        args
                    )
                );
            }

            /*
             * This handles normal method calls, i.e.
             * 
             * foo.bar();
             * 
             * Because we know that 'bar' has to be a method
             * on 'foo' here we can optimize this case during
             * compile time, instead of runtime
             * */
            if (Target is MemberAccessNode)
            {
                var target = (Ast.MemberAccessNode)Target;
                var tmp = Et.Variable(typeof(object), "#tmp");
                var targetExpr = etgen.GenerateConvertToObject(
                        target.Target.Generate(etgen)
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

            /*
             * This handles all other function invocations
             * 
             * foo();               // when outside a with-statement
             * (function(){})();    // inline functions
             * */
            return Et.Dynamic(
                etgen.Context.CreateInvokeBinder(
                    new CallInfo(args.Length)
                ),
                typeof(object),
                ArrayUtils.Insert(
                    Target.Generate(etgen),
                    Et.Property(
                        etgen.GlobalScopeExpr,
                        Scope.PiJsObject
                    ),
                    args
                )
            );
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
    }
}
