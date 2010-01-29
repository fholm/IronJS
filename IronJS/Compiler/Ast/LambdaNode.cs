using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class LambdaNode : Node
    {
        public List<string> Args { get; protected set; }
        public Node Body { get; protected set; }
        public string Name { get; protected set; }

        public LambdaNode(List<string> args, Node body, string name)
            : base(NodeType.Lambda)
        {
            Args = args;
            Body = body;
            Name = name;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type + " " + Name);
            var argsIndentStr = new String(' ', (indent + 1) * 2);
            writer.Append(argsIndentStr + "(Args");

            foreach (var node in Args)
                writer.Append(" " + node);

            writer.AppendLine(")");
            Body.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }

        public override Et Walk(EtGenerator etgen)
        {
            etgen.EnterFunctionScope();

            etgen.LambdaTuples.Add(
                Tuple.Create(
                    Et.Lambda<LambdaType>(
                        Et.Block(
                            // lambda body
                            Body.Walk(etgen),
                            Et.Label(
                                etgen.FunctionScope.ReturnLabel,
                                Undefined.Expr // 12.9
                            )
                        ),
                        etgen.FunctionScope.ScopeExpr
                    ),
                    // parameter names
                    Args
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
    }
}
