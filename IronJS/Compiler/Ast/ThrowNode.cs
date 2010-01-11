using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    class ThrowNode : Node
    {
        public readonly Ast.Node Target;

        public ThrowNode(Ast.Node target)
            : base(NodeType.Throw)
        {
            Target = target;
        }

        public override System.Linq.Expressions.Expression Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
