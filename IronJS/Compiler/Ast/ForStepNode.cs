using System;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    // 12.6.3
    class ForStepNode : LoopNode
    {
        public readonly Node Setup;
        public readonly Node Test;
        public readonly Node Incr;
        public readonly Node Body;

        public ForStepNode(Node setup, Node test, Node incr, Node body)
            : base(NodeType.ForStep)
        {
            Setup = setup;
            Test = test;
            Incr = incr;
            Body = body;
        }

        public override Et LoopWalk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
