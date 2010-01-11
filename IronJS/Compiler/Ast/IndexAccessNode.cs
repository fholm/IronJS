using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    class IndexAccessNode : Node
    {
        public readonly Node Target;
        public readonly Node Index;

        public IndexAccessNode(Node target, Node index)
            : base(NodeType.IndexAccess)
        {
            Target = target;
            Index = index;
        }

        public override System.Linq.Expressions.Expression Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
