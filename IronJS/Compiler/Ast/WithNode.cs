using IronJS.Runtime;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

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
            return Et.Block(
                Et.Assign(etgen.FunctionScope.ScopeExpr,
                    Scope.EtNewPrivate(
                        etgen.FunctionScope.ScopeExpr,
                        Target.Walk(etgen)
                    )
                ),
                Body.Walk(etgen),
                Et.Assign(
                    etgen.FunctionScope.ScopeExpr,
                    Scope.EtExit(
                        etgen.FunctionScope.ScopeExpr
                    )
                )
            );
        }
    }
}
