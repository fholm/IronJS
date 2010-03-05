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
		public INode Target { get { return Children[0]; } }
        public string Name { get; protected set; }

        public Member(INode target, string name, ITree node)
            : base(NodeType.MemberAccess, node)
        {
			Children = new[] { target };
            Name = name;
        }

        public override INode Analyze(Stack<Function> stack)
        {
			base.Analyze(stack);
			AnalyzeTools.IfIdentiferUsedAs(Target, IjsTypes.Object);
            return this;
        }
    }
}
