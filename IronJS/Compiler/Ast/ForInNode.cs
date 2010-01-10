using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    // 12.6.4
    class ForInNode : Node
    {
        public readonly Node Target;
        public readonly Node Source;
        public readonly Node Body;

        public ForInNode(Node target, Node source, Node body)
            : base(NodeType.ForIn)
        {
            Target = target;
            Source = source;
            Body = body;
        }

        public override System.Linq.Expressions.Expression Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
