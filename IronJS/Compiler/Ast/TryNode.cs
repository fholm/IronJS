using IronJS.Runtime;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class TryNode : Node
    {
        public readonly Node Body;
        public readonly CatchNode Catch;
        public readonly Node Finally;

        public TryNode(Node body, CatchNode _catch, Node _finally)
            : base(NodeType.Try)
        {
            Body = body;
            Catch = _catch;
            Finally = _finally;
        }

        public override Et Walk(EtGenerator etgen)
        {
            // try ... finally
            if (Catch == null)
            {
                return Et.TryFinally(
                    Body.Walk(etgen),
                    Finally.Walk(etgen)
                );
            }
            else
            {
                var catchParam = Et.Parameter(typeof(JsRuntimeError), "#catch");

                var catchBody = Et.Block(
                    etgen.GenerateAssign(
                        Catch.Target,
                        Et.Property(
                            catchParam,
                            "JsObj"
                        )
                    ),
                    Et.Block(
                        Catch.Body.Walk(etgen)
                    )
                );

                var catchBlock = Et.Catch(
                    catchParam,
                    EtUtils.Cast<object>(catchBody)
                );

                var tryBody = EtUtils.Box(Body.Walk(etgen));

                // try ... catch 
                if (Finally == null)
                {
                    return Et.TryCatch(
                        tryBody,
                        catchBlock
                    );
                }
                // try ... catch ... finally
                else
                {
                    return Et.TryCatchFinally(
                        tryBody,
                        Finally.Walk(etgen),
                        catchBlock
                    );
                }
            }
        }
    }
}
