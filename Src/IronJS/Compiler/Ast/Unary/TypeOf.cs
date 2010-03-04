using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using System.Collections.Generic;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    public class TypeOf : Node
    {
        public INode Target { get; protected set; }

        public TypeOf(INode target, ITree node)
            : base(NodeType.TypeOf, node)
        {
            Target = target;
        }

        public override Type Type
        {
            get
            {
                return IjsTypes.String;
            }
        }

        public override INode Analyze(Stack<Function> stack)
        {
            Target = Target.Analyze(stack);
            return this;
        }
    }
}
