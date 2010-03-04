using System;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using System.Text;
using System.Collections.Generic;
using IronJS.Compiler.Tools;

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
            Property = Target.Analyze(astopt);

			AnalyzeTools.IfIdentiferUsedAs(Target, IjsTypes.Object);

            return this;
        }
    }
}
