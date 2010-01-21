using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class LambdaNode : Node
    {
        public readonly List<IdentifierNode> Args;
        public readonly Node Body;
        public readonly string Name;

        public LambdaNode(List<IdentifierNode> args, Node body, string name)
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
            writer.AppendLine(argsIndentStr + "(Args");

            foreach (var node in Args)
                node.Print(writer, indent + 2);

            writer.AppendLine(argsIndentStr + ")");
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
                    Args.Select(x => x.Name).ToList()
                )
            );

            etgen.ExitFunctionScope();

            return Context.EtCreateFunction(
                etgen.Context,
                etgen.FunctionScope.ScopeExpr,
                FunctionTable.EtPull(
                    etgen.FuncTableExpr,
                    etgen.LambdaId
                )
            );
        }
    }
}
