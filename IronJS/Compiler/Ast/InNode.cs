using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Et = System.Linq.Expressions.Expression;
using IronJS.Runtime.Utils;
using IronJS.Runtime.Js;

namespace IronJS.Compiler.Ast
{
    class InNode : Node
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
