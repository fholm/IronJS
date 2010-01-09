using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    using Et = System.Linq.Expressions.Expression;

    class ReturnNode : Node
    {
        public readonly Node Value;

        public ReturnNode(Node value)
            : base(NodeType.Return)
        {
            Value = value;
        }

        public override Et Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
