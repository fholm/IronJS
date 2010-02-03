using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
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

        public override INode Analyze(FuncNode astopt)
        {
            if (Setup != null)
                Setup = Setup.Analyze(astopt);

            if(Test != null)
                Test = Test.Analyze(astopt);

            if (Incr != null)
                Incr = Incr.Analyze(astopt);

            if (Body != null)
                Body = Body.Analyze(astopt);

            return this;
        }

        public override Et EtGen(FuncNode func)
        {
            Et test = AstUtils.Empty();
            Et setup = AstUtils.Empty();
            Et incr = AstUtils.Empty();

            if (Setup != null)
                setup = Setup.EtGen(func);

            if (Test != null)
                if (Test.ExprType == IjsTypes.Boolean)
                    test = Test.EtGen(func);
                else
                    throw new NotImplementedException();

            else
                test = Et.Constant(true, typeof(bool));

            if (Incr != null)
                incr = Incr.EtGen(func);

            var tmp = Et.Variable(typeof(Func<object>), "lambda");

            return Et.Block(
                new[] { tmp },
                Et.Assign(
                    tmp,
                    Et.Lambda(Body.EtGen(func))
                ),
                setup,
                AstUtils.Loop(
                    test,
                    incr,
                    Et.Invoke(
                        tmp
                    ),
                    AstUtils.Empty()
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
