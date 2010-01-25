using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class InstanceOfNode : Node
    {
        public Node Target { get; protected set; }
        public Node Function { get; protected set; }

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
