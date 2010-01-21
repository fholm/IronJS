using System;
using Et = System.Linq.Expressions.Expression;
using AstUtils = Microsoft.Scripting.Ast.Utils;

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
