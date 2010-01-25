using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class InstanceOfNode : Node
    {
        public readonly Node Target;
        public readonly Node Function;

        public InstanceOfNode(Node target, Node function)
            : base(NodeType.InstanceOf)
        {
            Target = target;
            Function = function;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Call(
                typeof(Operators).GetMethod("InstanceOf"),
                Target.Walk(etgen),
                Function.Walk(etgen)
            );
        }
    }
}
