using System;

namespace IronJS.Compiler.Ast
{
    public class CatchNode : Node
    {
        public Node Target { get; protected set; }
        public Node Body { get; protected set; }

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
