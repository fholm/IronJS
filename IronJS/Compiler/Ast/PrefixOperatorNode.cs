using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace IronJS.Compiler.Ast
{
    class PostfixOperatorNode : Node
    {
        public readonly Ast.Node Target;
        public readonly ExpressionType Op;

        public PostfixOperatorNode(Ast.Node node, ExpressionType op)
            : base(NodeType.PostfixOperator)
        {
            Target = node;
            Op = op;
        }

        public override Expression Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
