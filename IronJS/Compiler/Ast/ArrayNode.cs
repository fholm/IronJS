using System;
using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime.Tree;
using IronJS.Runtime;
using Et = System.Linq.Expressions.Expression;

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

        public override INode Analyze(IjsAstAnalyzer astopt)
        {
            for (int i = 0; i < Values.Count; ++i)
                Values[i] = Values[i].Analyze(astopt);

            return this;
        }
    }
}
