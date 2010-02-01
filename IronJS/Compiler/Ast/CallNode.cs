using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Optimizer;
using IronJS.Runtime.Binders2;
using IronJS.Runtime.Utils;
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

        public override INode Analyze(IjsAstAnalyzer astopt)
        {
            Target = Target.Analyze(astopt);

            var args = new List<INode>();

            foreach (var arg in Args)
                args.Add(arg.Analyze(astopt));

            Args = args;

            var idNode = Target as IdentifierNode;
            if (idNode != null && idNode.IsLocal)
                idNode.VarInfo.UsedAs.Add(typeof(IjsFunc));

            return this;
        }

        public override Et EtGen(IjsEtGenerator etgen)
        {
            var idNode = Target as IdentifierNode;
            if (idNode != null && !idNode.IsGlobal)
            {
                IjsFuncInfo funcInfo;
                if(idNode.VarInfo.GetFuncInfo(out funcInfo))
                {
                    if (funcInfo.IsCompiled)
                    {
                        return Et.Call(
                            funcInfo.CompiledMethod,
                            EtUtils.ConvertToParamTypes(
                                new[] { etgen.GlobalsExpr }.Concat(
                                    Args.Select(x => x.EtGen(etgen))
                                ),
                                funcInfo.CompiledMethod.GetParameters()
                            )
                        );
                    }
                }

                throw new NotImplementedException();
            }
            else
            {
                if (idNode.Name == "time")
                {
                    return Et.Call(
                        typeof(HelperFunctions).GetMethod("Timer"),
                        Args[0].EtGen(etgen),
                        etgen.GlobalsExpr
                    );
                }

                var delegateType = typeof(Func<CallSite, object, object, object>);

                var callSite = CallSite.Create(
                    delegateType,
                    new JsInvokeBinder2(
                        new CallInfo(Args.Count)
                    )
                 );

                var serializer = callSite.Binder as Microsoft.Scripting.Runtime.IExpressionSerializable;
                var siteType = callSite.GetType();
                var field = etgen.CreateCallSiteField<Func<CallSite, object, object, object>>();
                var init = Expression.Call(siteType.GetMethod("Create"), serializer.CreateExpression());

                return Expression.Block(
                    Et.IfThen(
                        Et.Equal(
                            Et.Constant(null, typeof(object)),
                            field
                        ),
                        Expression.Assign(
                            field, init
                        )
                    ),
                    Expression.Call(
                        Expression.Field(
                            field,
                            siteType.GetField("Target")
                        ),
                        delegateType.GetMethod("Invoke"),
                        field,
                        idNode.EtGen(etgen),
                        etgen.GlobalsExpr
                    )
                );
            }

            throw new NotImplementedException();
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);
            Target.Print(writer, indent + 1);

            var indentStr2 = new String(' ', (indent + 1) * 2);

            if (Args.Count > 0)
            {
                writer.AppendLine(indentStr2 + "(Args");

                foreach (var node in Args)
                    node.Print(writer, indent + 2);

                writer.AppendLine(indentStr2 + ")");
            }
            else
            {
                writer.AppendLine(indentStr2 + "(Args)");
            }

            writer.AppendLine(indentStr + ")");
        }
    }
}
