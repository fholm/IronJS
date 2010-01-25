using System;
using Et = System.Linq.Expressions.Expression;
using AstUtils = Microsoft.Scripting.Ast.Utils;

namespace IronJS.Compiler.Ast
{
    // 12.6.3
    public class ForStepNode : LoopNode
    {
        public Node Setup { get; protected set; }
        public Node Test { get; protected set; }
        public Node Incr { get; protected set; }
        public Node Body { get; protected set; }

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
            return Et.Block(
                Setup.Walk(etgen),
                AstUtils.Loop(
                    Et.Dynamic(
                        etgen.Context.CreateConvertBinder(typeof(bool)),
                        typeof(bool),
                        Test.Walk(etgen)
                    ),
                    Incr.Walk(etgen),
                    Body.Walk(etgen),
                    null,
                    etgen.FunctionScope.LabelScope.Break(),
                    etgen.FunctionScope.LabelScope.Continue()
                )
            );
        }
    }
}
