using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;

namespace IronJS.Compiler.Ast
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;
    using System.Text;

    public class InNode : Node
    {
        public INode Target { get; protected set; }
        public INode Property { get; protected set; }

        public InNode(INode target, INode property, ITree node)
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

        public override INode Analyze(FuncNode astopt)
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
