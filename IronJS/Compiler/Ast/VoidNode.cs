using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    class VoidNode : Node
    {
        public readonly Ast.Node Target;

        public VoidNode(Ast.Node target)
            : base(NodeType.Void)
        {
            Target = target;
        }

        public override System.Linq.Expressions.Expression Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
