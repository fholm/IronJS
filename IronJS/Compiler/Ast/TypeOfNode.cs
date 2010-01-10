using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    class TypeOfNode : Node
    {
        public readonly Node Target;

        public TypeOfNode(Node target)
            : base(NodeType.TypeOf)
        {
            Target = target;
        }
        
        public override System.Linq.Expressions.Expression Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
