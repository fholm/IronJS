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

        public override INode Analyze(Function astopt)
        {
            Target = Target.Analyze(astopt);
            Value = Value.Analyze(astopt);

            IfIdentiferUsedAs(Target, IjsTypes.Object);

            return this;
        }
        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);
            Value.Print(writer, indent+1);
            Target.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}
