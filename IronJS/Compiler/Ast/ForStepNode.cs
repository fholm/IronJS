using System;
using Et = System.Linq.Expressions.Expression;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using IronJS.Runtime.Utils;

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
                Et.Assign(
                    etgen.Tmp,
                    Et.Constant(1, typeof(object))
                ),
                AstUtils.Loop(
                    Et.LessThan(
                        Et.Convert(etgen.Tmp, typeof(int)),
                        Et.Constant(10000000, typeof(int))
                    ),
                    Et.Assign(
                        etgen.Tmp,
                        EtUtils.Box(Et.Add(
                            Et.Convert(etgen.Tmp, typeof(int)),
                            Et.Constant(1)
                        ))
                    ),
                    AstUtils.Empty(),
                    AstUtils.Empty()
                )
            );

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
