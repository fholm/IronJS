using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    class UnsignedRightShiftNode : Node
    {
        public readonly Ast.Node Left;
        public readonly Ast.Node Right;

        public UnsignedRightShiftNode(Ast.Node left, Ast.Node right)
            : base(NodeType.UnsignedRightShift)
        {
            Left = left;
            Right = right;
        }

        public override System.Linq.Expressions.Expression Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
