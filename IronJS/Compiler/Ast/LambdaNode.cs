using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class LambdaNode : Node
    {
        public IdentifierNode Name { get; protected set; }
        public List<IdentifierNode> Args { get; protected set; }
        public INode Body { get; protected set; }
        public bool IsLambda { get { return Name == null; } }

        public LambdaNode(List<IdentifierNode> args, INode body, IdentifierNode name, ITree node)
            : base(NodeType.Lambda, node)
        {
            Args = args;
            Body = body;
            Name = name;
        }

        public override JsType ExprType
        {
            get
            {
                return JsType.Object;
            }
        }

        public override INode Optimize(AstOptimizer astopt)
        {
            if (!astopt.IsGlobal)
            {
                if (!IsLambda)
                    astopt.Scope.CreateVariable(Name.Name).AssignedFrom.Add(this);
            }

            astopt.EnterScope();

            foreach (var arg in Args)
                astopt.Scope.CreateVariable(arg.Name).UsedAs.Add(JsType.Dynamic);

            Body = Body.Optimize(astopt);

            astopt.ExitScope();
            return this;
        }

        public override Et Generate2(EtGenerator etgen)
        {
            etgen.Enter();

            var lambda = Et.Lambda(
                Et.Block(
                    etgen.LambdaScope.Variables.Values,
                    Body.Generate2(etgen)
                )
            );

            etgen.LambdaScope = etgen.LambdaScope.Exit();

            if (IsLambda)
            {
                return lambda;
            }
            else
            {
                return etgen.GenerateAssign2(Name, lambda);
            }
        }

        public override Et Generate(EtGenerator etgen)
        {
            etgen.EnterFunctionScope();

            etgen.LambdaTuples.Add(
                Tuple.Create(
                    Et.Lambda<LambdaType>(
                        Et.Block(
                            // lambda body
                            Body.Generate(etgen),
                            Et.Label(
                                etgen.FunctionScope.ReturnLabel,
                                Undefined.Expr // 12.9
                            )
                        ),
                        etgen.FunctionScope.ScopeExpr
                    ),
                    // parameter names
                    Args.Select(x => x.Name).ToList()
                )
            );

            etgen.ExitFunctionScope();

            if (Name == null)
            {
                return Et.Call(
                    Et.Constant(etgen.Context),
                    Context.MiCreateFunction,
                    etgen.FunctionScope.ScopeExpr,
                    FunctionTable.EtPull(
                        etgen.FuncTableExpr,
                        etgen.LambdaId
                    )
                );
            }
            else
            {
                var tmp = Et.Parameter(typeof(IFunction), "#tmp");

                return Et.Block(
                    new[] { tmp },
                    Et.Assign(
                        tmp,
                        Et.Call(
                            Et.Constant(etgen.Context),
                            Context.MiCreateFunction,
                            etgen.FunctionScope.ScopeExpr,
                            FunctionTable.EtPull(
                                etgen.FuncTableExpr,
                                etgen.LambdaId
                            )
                        )
                    ),
                    Et.Call(
                        etgen.FunctionScope.ScopeExpr,
                        Scope.MiLocal,
                        Et.Constant(Name, typeof(object)),
                        tmp
                    ),
                    tmp
                );
            }
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType + " " + Name);
            var argsIndentStr = new String(' ', (indent + 1) * 2);
            writer.Append(argsIndentStr + "(Args");

            foreach (var node in Args)
                writer.Append(" " + node);

            writer.AppendLine(")");
            Body.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}
