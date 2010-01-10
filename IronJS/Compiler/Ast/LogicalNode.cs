using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace IronJS.Compiler.Ast
{
    class LogicalNode : Node
    {
        public readonly Ast.Node Left;
        public readonly Ast.Node Right;
        public readonly ExpressionType Op;

        public LogicalNode(Node left, Node right, ExpressionType op)
            : base(NodeType.Logical)
        {
            Left = left;
            Right = right;
            Op = op;
        }

        public override Expression Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
