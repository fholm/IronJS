using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    using Et = System.Linq.Expressions.Expression;

    enum UnaryOp { PostInc, PostDec, Dec, Inc, Inv, Not, Negate }

    class UnaryOpNode : Node
    {
        public readonly Node Target;
        public readonly UnaryOp Op;

        public UnaryOpNode(Node target, UnaryOp op)
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
