using System;

namespace IronJS.Compiler.Ast
{
    using Et = System.Linq.Expressions.Expression;

    class WithNode : Node
    {
        public readonly Node Target;
        public readonly Node Body;

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
