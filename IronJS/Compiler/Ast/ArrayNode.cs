using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime.Tree;
using IronJS.Runtime;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class ArrayNode : Node
    {
        public List<Node> Values { get; protected set; }

        public ArrayNode(List<Node> values, ITree node)
            : base(NodeType.Array, node)
        {
            Values = values;
        }

        public override Et Generate(EtGenerator etgen)
        {
            return Et.Call(
                Et.Constant(etgen.Context),
                Context.MiCreateArray,
                Et.NewArrayInit(
                    typeof(object),
                    Values.Select(x => x.Generate(etgen))
                )
            );
        }
    }
}
