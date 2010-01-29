using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class WithNode : Node
    {
        public INode Target { get; protected set; }
        public INode Body { get; protected set; }

        public WithNode(INode target, INode body, ITree node)
            : base(NodeType.With, node)
        {
            Target = target;
            Body = body;
        }

        public override Et Generate(EtGenerator etgen)
        {
            etgen.EnterWith();
            var body = Body.Generate(etgen);
            etgen.ExitWith();

            return Et.Block(
                Et.Assign(etgen.FunctionScope.ScopeExpr,
                    AstUtils.SimpleNewHelper(
                        Scope.Ctor2Args,
                        etgen.FunctionScope.ScopeExpr,
                        Target.Generate(etgen)
                    )
                ),
                body,
                Et.Assign(
                    etgen.FunctionScope.ScopeExpr,
                    Et.Property(
                        etgen.FunctionScope.ScopeExpr,
                        Scope.PiParentScope
                    )
                )
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);

            Target.Print(writer, indent + 1);
            Body.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
