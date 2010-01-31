using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Optimizer;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class IjsFunc
    {
        public MethodInfo MethodInfo;

        public IjsFunc(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
        }
    }

    public class IjsFunc<T> : IjsFunc
    {
        public T Closure;

        public IjsFunc(MethodInfo methodInfo, T closure)
            : base(methodInfo)
        {
            Closure = closure;
        }
    }

    public class FuncNode : Node
    {
        public INode Body { get; protected set; }
        public IdentifierNode Name { get; protected set; }
        public List<IdentifierNode> Args { get; protected set; }
        public IjsFuncInfo FuncInfo { get; protected set; }

        public FuncNode(List<IdentifierNode> args, INode body, IdentifierNode name, ITree node)
            : base(NodeType.Func, node)
        {
            Args = args;
            Body = body;
            Name = name;

            FuncInfo = new IjsFuncInfo(this);
            FuncInfo.IsLambda = Name == null;

            if (Name != null)
                Name.IsDefinition = true;
        }

        public override Type ExprType
        {
            get
            {
                return FuncInfo.ExprType;
            }
        }

        public override INode Analyze(IjsAstAnalyzer analyzer)
        {
            if (!analyzer.InGlobalScope)
            {
                if (!FuncInfo.IsLambda)
                {
                    Name.Analyze(analyzer);
                    Name.VarInfo.AssignedFrom.Add(this);
                    Name.VarInfo.UsedAs.Add(ExprType);
                }
            }

            analyzer.EnterScope(FuncInfo);

            foreach (var arg in Args)
            {
                arg.Analyze(analyzer);
                arg.VarInfo.IsParameter = true;
            }

            Body = Body.Analyze(analyzer);

            analyzer.ExitScope();
            return this;
        }

        public override Et EtGen(IjsEtGenerator etgen)
        {
            FuncInfo.CompiledMethod = 
                etgen.CompileFunction(
                    Args,
                    Body,
                    FuncInfo
                );

            if (FuncInfo.IsLambda)
            {
                if (FuncInfo.ClosureType == null)
                {
                    return Et.New(
                        typeof(IjsFunc).GetConstructor(new[] { typeof(MethodInfo) }),
                        etgen.CreateMethodInfoField(FuncInfo.CompiledMethod.DeclaringType.Name)
                    );
                }
                else
                {
                    var ijsFuncType = typeof(IjsFunc<>).MakeGenericType(FuncInfo.ClosureType);

                    var tmp = Et.Variable(FuncInfo.ClosureType, "$tmp");

                    return Et.Block(
                        new[] { tmp },
                        Et.Assign(
                            tmp,
                            Et.New(
                                FuncInfo.ClosureType.GetConstructor(Type.EmptyTypes)
                            )
                        ),
                        Et.Assign(
                            Et.Field(
                                tmp,
                                FuncInfo.ClosureType.GetField("z")
                            ),
                            etgen.Scope["z"].Item1
                        ),
                        Et.New(
                            ijsFuncType.GetConstructor(new[] { typeof(MethodInfo), FuncInfo.ClosureType }),
                            etgen.CreateMethodInfoField(FuncInfo.CompiledMethod.DeclaringType.Name),
                            tmp
                        )
                    );
                }
            }

            throw new NotImplementedException();
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

            writer.AppendLine(indentStr + "(" + NodeType + (" " + Name + " ").TrimEnd() + " " + FuncInfo.ReturnType);
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
