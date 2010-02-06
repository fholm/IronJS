using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;
    using System.Text;

    public class ForIn : Node
    {
        public INode Target { get; protected set; }
        public INode Source { get; protected set; }
        public INode Body { get; protected set; }

        public ForIn(INode target, INode source, INode body, ITree node)
            : base(NodeType.ForIn, node)
        {
            Target = target;
            Source = source;
            Body = body;
        }

        public override INode Analyze(Stack<Function> astopt)
        {
            Target = Target.Analyze(astopt);
            Source = Source.Analyze(astopt);
            Body = Body.Analyze(astopt);

            IfIdentiferUsedAs(Source, IjsTypes.Object);

            return this;
        }

        public override void Write(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            Target.Write(writer, indent + 1);
            Source.Write(writer, indent + 1);
            Body.Write(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
