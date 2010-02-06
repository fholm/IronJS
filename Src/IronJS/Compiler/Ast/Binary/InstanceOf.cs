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

        public override Type ExprType
        {
            get
            {
                return IjsTypes.Boolean;
            }
        }

        public override INode Analyze(Function astopt)
        {
            Target = Target.Analyze(astopt);
            Function = Target.Analyze(astopt);

            IfIdentiferUsedAs(Target, IjsTypes.Object);
            IfIdentiferUsedAs(Function, IjsTypes.Object);

            return this;
        }

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            Target.Print(writer, indent + 1);
            Function.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
