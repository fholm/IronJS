using System;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using System.Text;

#if CLR2
using Microsoft.Scripting.Ast;
using System.Collections.Generic;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    public class If : Node
    {
        public INode Test { get; protected set; }
        public INode TrueBranch { get; protected set; }
        public INode ElseBranch { get; protected set; }
        public bool HasElseBranch { get { return ElseBranch != null; } }
        public bool IsTernary { get; protected set; }

        public If(INode test, INode trueBranch, INode elseBranch, bool isTernary, ITree node)
            : base(NodeType.If, node)
        {
            Test = test;
            TrueBranch = trueBranch;
            ElseBranch = elseBranch;
            IsTernary = isTernary;
        }

        public override Type Type
        {
            get
            {
                if (IsTernary)
                {
                    if (TrueBranch.Type == ElseBranch.Type)
                        return TrueBranch.Type;
                }

                return IjsTypes.Dynamic;
            }
        }

        public override INode Analyze(Stack<Function> astopt)
        {
            Test = Test.Analyze(astopt);
            TrueBranch = TrueBranch.Analyze(astopt);

            if(HasElseBranch)
                ElseBranch = ElseBranch.Analyze(astopt);

            return this;
        }
    }
}
