using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    class BooleanNode : Node
    {
        public readonly bool Value;

        public BooleanNode(bool value)
            : base(NodeType.Boolean)
        {
            Value = value;
        }

        public override System.Linq.Expressions.Expression Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
