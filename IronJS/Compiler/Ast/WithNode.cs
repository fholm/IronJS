using System;

namespace IronJS.Compiler.Ast
{
    using Et = System.Linq.Expressions.Expression;

    class WithNode : Node
    {
        private Node Target;
        private Node Body;

        public WithNode(Node target, Node body)
            : base(NodeType.With)
        {
            Target = target;
            Body = body;
        }

        public override Et Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
