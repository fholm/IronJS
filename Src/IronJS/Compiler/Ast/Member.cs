using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using System.Collections.Generic;
using IronJS.Compiler.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    public class Member : Node
    {
        public INode Target { get; protected set; }
        public string Name { get; protected set; }

        public Member(INode target, string member, ITree node)
            : base(NodeType.MemberAccess, node)
        {
            Target = target;
            Name = member;
        }

        public override INode Analyze(Stack<Function> astopt)
        {
			AnalyzeTools.IfIdentiferUsedAs(Target, IjsTypes.Object);
            return this;
        }
    }
}
