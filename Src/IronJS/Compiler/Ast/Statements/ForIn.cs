using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
using System.Text;
using IronJS.Compiler.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;

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

			AnalyzeTools.IfIdentiferUsedAs(Source, IjsTypes.Object);

            return this;
        }
    }
}
