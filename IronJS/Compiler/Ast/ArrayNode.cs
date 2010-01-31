using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime.Tree;
using IronJS.Runtime;
using Et = System.Linq.Expressions.Expression;
using System;

namespace IronJS.Compiler.Ast
{
    public class ArrayNode : Node, INode
    {
        public List<INode> Values { get; protected set; }

        public ArrayNode(List<INode> values, ITree node)
            : base(NodeType.Array, node)
        {
            Values = values;
        }

        public override Type ExprType
        {
            get
            {
                return IjsTypes.Object;
            }
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
