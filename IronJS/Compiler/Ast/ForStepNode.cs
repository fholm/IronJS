using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    class ForStepNode : Node
    {
        public readonly Node Init;
        public readonly Node Test;
        public readonly Node Incr;
        public readonly Node Body;

        public ForStepNode(Node init, Node test, Node incr, Node body)
            : base(NodeType.ForStep)
        {
            Init = init;
            Test = test;
            Incr = incr;
            Body = body;
        }

        public override System.Linq.Expressions.Expression Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
