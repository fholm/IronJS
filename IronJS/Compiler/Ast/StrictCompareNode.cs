using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    class StrictCompareNode : Node
    {
        public readonly Ast.Node Left;
        public readonly Ast.Node Right;
        public readonly ExpressionType Op;

        public StrictCompareNode(Ast.Node left, Ast.Node right, ExpressionType op)
            : base(NodeType.StrictCompare)
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
