using System;
using System.Linq.Expressions;
using Antlr.Runtime.Tree;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    // 12.6.3
    public class ForStepNode : LoopNode
    {
        public Node Setup { get; protected set; }
        public Node Test { get; protected set; }
        public Node Incr { get; protected set; }
        public Node Body { get; protected set; }

        public ForStepNode(Node setup, Node test, Node incr, Node body, ITree node)
            : base(NodeType.ForStep, node)
        {
            Setup = setup;
            Test = test;
            Incr = incr;
            Body = body;
        }

        public override Node Optimize(AstOptimizer astopt)
        {
            if (Setup != null)
                Setup = Setup.Optimize(astopt);

            if(Test != null)
                Test = Test.Optimize(astopt);

            if (Incr != null)
                Incr = Incr.Optimize(astopt);

            if (Body != null)
                Body = Body.Optimize(astopt);

            return this;
        }

        public override Et LoopWalk(EtGenerator etgen)
        {
            return Et.Block(
                Setup.Generate(etgen),
                AstUtils.Loop(
                    Et.Dynamic(
                        etgen.Context.CreateConvertBinder(typeof(bool)),
                        typeof(bool),
                        Test.Generate(etgen)
                    ),
                    Incr.Generate(etgen),
                    Body.Generate(etgen),
                    null,
                    etgen.FunctionScope.LabelScope.Break(),
                    etgen.FunctionScope.LabelScope.Continue()
                )
            );
        }

        public override void Print(System.Text.StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);

            Setup.Print(writer, indent + 1);
            Test.Print(writer, indent + 1);
            Incr.Print(writer, indent + 1);
            Body.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
