using System;

namespace IronJS.Compiler.Ast
{
    class CatchNode : Node
    {
        public readonly Node Target;
        public readonly Node Body;

        public CatchNode(Node target, Node body)
            : base(NodeType.Catch)
        {
            Target = target;
            Body = body;
        }

        public override System.Linq.Expressions.Expression Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
