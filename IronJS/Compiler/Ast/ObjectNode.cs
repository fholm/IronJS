using System.Collections.Generic;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class ObjectNode : Node
    {
        public Dictionary<string, Node> Properties { get; protected set; }

        public ObjectNode(Dictionary<string, Node> properties)
            : base(NodeType.Object)
        {
            Properties = properties;
        }

        public override Et Walk(EtGenerator etgen)
        {
            var tmp = Et.Parameter(typeof(IObj), "#tmp");
            var exprs = new List<Et>();

            foreach (var kvp in Properties)
            {
                exprs.Add(
                    Et.Call(
                        IObjUtils.MiSetObj,
                        tmp,
                        Et.Constant(kvp.Key, typeof(object)),
                        kvp.Value.Walk(etgen)
                    )
                );
            }

            return Et.Block(
                new[] { tmp },
                Et.Assign(
                    tmp,
                    Et.Call(
                        Et.Constant(etgen.Context),
                        Context.MiCreateObject
                    )
                ),
                Et.Block(
                    exprs
                ),
                tmp
            );
        }
    }
}
