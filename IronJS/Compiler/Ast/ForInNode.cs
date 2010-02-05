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

    public class ForInNode : LoopNode
    {
        public INode Target { get; protected set; }
        public INode Source { get; protected set; }
        public INode Body { get; protected set; }

        public ForInNode(INode target, INode source, INode body, ITree node)
            : base(NodeType.ForIn, node)
        {
            Target = target;
            Source = source;
            Body = body;
        }

        public override INode Analyze(FuncNode astopt)
        {
            Target = Target.Analyze(astopt);
            Source = Source.Analyze(astopt);
            Body = Body.Analyze(astopt);

            IfIdentiferUsedAs(Source, IjsTypes.Object);

            return this;
        }

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            Target.Print(writer, indent + 1);
            Source.Print(writer, indent + 1);
            Body.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
