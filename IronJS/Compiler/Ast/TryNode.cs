using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    class TryNode : Node
    {
        public readonly Node Body;
        public readonly CatchNode Catch;
        public readonly Node Finally;

        public TryNode(Node body, CatchNode _catch, Node _finally)
            : base(NodeType.Try)
        {
            Body = body;
            Catch = _catch;
            Finally = _finally;
        }

        public override System.Linq.Expressions.Expression Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
