using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    class ContinueNode : Node
    {
        public readonly string Label;

        public ContinueNode()
            : this(null)
        {

        }

        public ContinueNode(string label)
            : base(NodeType.Continue)
        {
            Label = label;
        }

        public override System.Linq.Expressions.Expression Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
