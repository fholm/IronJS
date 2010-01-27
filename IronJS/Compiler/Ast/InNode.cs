using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class InNode : Node
    {
        public Node Target { get; protected set; }
        public Node Property { get; protected set; }

        public InNode(Node target, Node property)
            : base(NodeType.In)
        {
            Target = target;
            Property = property;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Call(
                EtUtils.Cast<IObj>(Target.Walk(etgen)),
                IObjUtils.MiHas,
                Property.Walk(etgen)
            );
        }
    }
}
