using System.Linq;
using System.Collections.Generic;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class ArrayNode : Node
    {
        public List<Node> Values { get; protected set; }

        public ArrayNode(List<Node> values)
            : base(NodeType.Array)
        {
            Values = values;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Call(
                Et.Constant(etgen.Context),
                Context.MiCreateArray,
                Et.NewArrayInit(
                    typeof(object),
                    Values.Select(x => x.Walk(etgen))
                )
            );
        }
    }
}
