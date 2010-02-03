using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;
using IronJS.Runtime2.Js;

namespace IronJS.Compiler.Ast
{
    // 12.6.4
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

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            Target.Print(writer, indent + 1);
            Source.Print(writer, indent + 1);
            Body.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
