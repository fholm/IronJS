using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
using System.Collections.Generic;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    public class InstanceOf : Node
    {
        public INode Target { get; protected set; }
        public INode Function { get; protected set; }

        public InstanceOf(INode target, INode function, ITree node)
            : base(NodeType.InstanceOf, node)
        {
            Target = target;
            Function = function;
        }

        public override Type Type
        {
            get
            {
                return IjsTypes.Boolean;
            }
        }

        public override INode Analyze(Stack<Function> astopt)
        {
            Target = Target.Analyze(astopt);
            Function = Target.Analyze(astopt);

            IfIdentiferUsedAs(Target, IjsTypes.Object);
            IfIdentiferUsedAs(Function, IjsTypes.Object);

            return this;
        }

        public override void Write(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            Target.Write(writer, indent + 1);
            Function.Write(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
