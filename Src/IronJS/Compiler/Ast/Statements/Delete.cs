using System;
using System.Collections.Generic;
using System.Text;
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

    public class Delete : Node
    {
        public INode Target { get; protected set; }

        public Delete(INode target, ITree node)
            : base(NodeType.Delete, node)
        {
            Target = target;
        }

        public override Type Type
        {
            get
            {
                return IjsTypes.Boolean;
            }
        }

        public override INode Analyze(Stack<Function> stack)
        {
            Target = Target.Analyze(stack);
            return this;
        }
    }
}
