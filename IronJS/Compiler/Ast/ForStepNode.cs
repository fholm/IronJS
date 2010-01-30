using System;
using System.Text;
using Antlr.Runtime.Tree;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    // 12.6.3
    public class ForStepNode : LoopNode
    {
        public INode Setup { get; protected set; }
        public INode Test { get; protected set; }
        public INode Incr { get; protected set; }
        public INode Body { get; protected set; }

        public ForStepNode(INode setup, INode test, INode incr, INode body, ITree node)
            : base(NodeType.ForStep, node)
        {
            Setup = setup;
            Test = test;
            Incr = incr;
            Body = body;
        }

        public override INode Optimize(AstOptimizer astopt)
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

        public override Et Generate2(EtGenerator etgen)
        {
            Et test = AstUtils.Empty();
            Et setup = AstUtils.Empty();
            Et incr = AstUtils.Empty();

            if (Setup != null)
                setup = Setup.Generate2(etgen);

            if (Test != null)
                if (Test.ExprType == JsType.Boolean)
                    test = Test.Generate2(etgen);
                else
                    test = Et.Dynamic(
                        etgen.Context.CreateConvertBinder(typeof(bool)),
                        typeof(bool),
                        Test.Generate2(etgen)
                    );
            else
                test = Et.Constant(true, typeof(bool));

            if (Incr != null)
                incr = Incr.Generate2(etgen);

            return Et.Block(
                setup,
                AstUtils.Loop(
                    test,
                    incr,
                    Body.Generate2(etgen),
                    AstUtils.Empty()
                )
            );
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

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            Setup.Print(writer, indent + 1);
            Test.Print(writer, indent + 1);
            Incr.Print(writer, indent + 1);
            Body.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
