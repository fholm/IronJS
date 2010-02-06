using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using System.Collections.Generic;

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
            IfIdentiferUsedAs(Target, IjsTypes.Object);
            return this;
        }

        public override void Write(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType + " " + Name);
            Target.Write(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}
