using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace IronJS.Compiler.Ast
{
    using Et = System.Linq.Expressions.Expression;

    class UnaryOpNode : Node
    {
        public readonly Node Target;
        public readonly ExpressionType Op;

        public UnaryOpNode(Node target, ExpressionType op)
            : base(NodeType.UnaryOp)
        {
            Target = target;
            Op = op;
        }

        public override Et Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
