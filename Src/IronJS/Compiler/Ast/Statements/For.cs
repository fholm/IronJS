using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;
    using System.Collections.Generic;

    public class For : Node
    {
        public INode Setup { get; protected set; }
        public INode Test { get; protected set; }
        public INode Incr { get; protected set; }
        public INode Body { get; protected set; }

        public For(INode setup, INode test, INode incr, INode body, ITree node)
            : base(NodeType.ForStep, node)
        {
            Setup = setup;
            Test = test;
            Incr = incr;
            Body = body;
        }

        public override INode Analyze(Stack<Function> astopt)
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
    }
}
