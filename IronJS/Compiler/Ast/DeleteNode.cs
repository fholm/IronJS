using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    class DeleteNode : Node
    {
        public readonly Node Target;

        public DeleteNode(Node target)
            : base(NodeType.Delete)
        {
            Target = target;
        }

        public override System.Linq.Expressions.Expression Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
