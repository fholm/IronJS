using System;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using System.Text;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{

    public class In : Node
    {
        public INode Target { get; protected set; }
        public INode Property { get; protected set; }

        public In(INode target, INode property, ITree node)
            : base(NodeType.In, node)
        {
            Target = target;
            Property = property;
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
            Property = Target.Analyze(astopt);

            IfIdentiferUsedAs(Target, IjsTypes.Object);

            return this;
        }

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            Property.Print(writer, indent + 1);
            Target.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
