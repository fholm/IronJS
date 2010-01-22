using IronJS.Runtime;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;
using AstUtils = Microsoft.Scripting.Ast.Utils;

namespace IronJS.Compiler.Ast
{
    class WithNode : Node
    {
        public readonly Node Target;
        public readonly Node Body;

        public WithNode(Node target, Node body)
            : base(NodeType.With)
        {
            Target = target;
            Body = body;
        }

        public override Et Walk(EtGenerator etgen)
        {
            etgen.EnterWith();
            var body = Body.Walk(etgen);
            etgen.ExitWith();

            return Et.Block(
                Et.Assign(etgen.FunctionScope.ScopeExpr,
                    AstUtils.SimpleNewHelper(
                        Scope.Ctor2Args,
                        etgen.FunctionScope.ScopeExpr,
                        Target.Walk(etgen)
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
    }
}
