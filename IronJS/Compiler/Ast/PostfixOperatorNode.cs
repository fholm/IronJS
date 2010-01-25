using System.Linq.Expressions;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class PostfixOperatorNode : Node
    {
        public Node Target { get; protected set; }
        public ExpressionType Op { get; protected set; }

        public PostfixOperatorNode(Node node, ExpressionType op)
            : base(NodeType.PostfixOperator)
        {
            Target = node;
            Op = op;
        }

        public override Et Walk(EtGenerator etgen)
        {
            var tmp = Et.Parameter(typeof(double), "#tmp");

            return Et.Block(
                new[] { tmp },

                // the value we will return
                Et.Assign(
                    tmp,
                    Et.Dynamic(
                        etgen.Context.CreateConvertBinder(typeof(double)),
                        typeof(double),
                        Target.Walk(etgen)
                    )
                ),

                // calc new value
                etgen.GenerateAssign(
                    Target,
                    EtUtils.Box(
                        Et.Add(
                            tmp,
                            Et.Constant(
                                Op == ExpressionType.PostIncrementAssign
                                         ? 1.0    // 11.3.1
                                         : -1.0,  // 11.3.2
                                typeof(double)
                            )
                        )
                    )
                ),

                tmp // return the old value
            );
        }
    }
}
