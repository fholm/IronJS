using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class InNode : Node
    {
        private Node Target;
        private Node Property;

        public InNode(Node target, Node property)
            : base(NodeType.In)
        {
            Target = target;
            Property = property;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return IObjUtils.EtHasProperty(
                EtUtils.Cast<IObj>(Target.Walk(etgen)),
                Property.Walk(etgen)
            );
        }
    }
}
