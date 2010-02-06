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
    public class Index : Node
    {
        public INode Target { get; protected set; }
        public INode Value { get; protected set; }

        public Index(INode target, INode index, ITree node)
            : base(NodeType.IndexAccess, node)
        {
            Target = target;
            Value = index;
        }

        public override INode Analyze(Stack<Function> stack)
        {
            Target = Target.Analyze(stack);
            Value = Value.Analyze(stack);

            IfIdentiferUsedAs(Target, IjsTypes.Object);

            return this;
        }
        public override void Write(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);
            Value.Write(writer, indent+1);
            Target.Write(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}
