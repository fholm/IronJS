using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    class BreakNode : Node
    {
        public readonly string Label;

        public BreakNode(string label)
            : base(NodeType.Break)
        {
            Label = label;
        }
    
        public override System.Linq.Expressions.Expression  Walk(EtGenerator etgen)
        {
         	throw new NotImplementedException();
        }
    }
}
