using IronJS.Runtime;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class InstanceOfNode : Node
    {
        private Node Target;
        private Node Function;

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
