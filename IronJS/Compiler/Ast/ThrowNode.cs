using IronJS.Runtime;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class ThrowNode : Node
    {
        public readonly Node Target;

        public ThrowNode(Node target)
            : base(NodeType.Throw)
        {
            Target = target;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Throw(
                AstUtils.SimpleNewHelper(
                    JsRuntimeError.Ctor,
                    etgen.GenerateConvertToObject(
                        Target.Walk(etgen)
                    )
                )
            );
        }
    }
}
