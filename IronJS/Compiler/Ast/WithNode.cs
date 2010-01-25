using IronJS.Runtime;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;
using AstUtils = Microsoft.Scripting.Ast.Utils;

namespace IronJS.Compiler.Ast
{
    public class WithNode : Node
    {
        public Node Target { get; protected set; }
        public Node Body { get; protected set; }

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
