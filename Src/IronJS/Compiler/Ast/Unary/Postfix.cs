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
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;
    using System.Collections.Generic;

    public class Postfix : Node
    {
        public INode Target { get; protected set; }
        public ExpressionType Op { get; protected set; }

        public Postfix(INode node, ExpressionType op, ITree tree)
            : base(NodeType.PostfixOperator, tree)
        {
            Target = node;
            Op = op;
        }

        public override Type Type
        {
            get
            {
                if (Target.Type == IjsTypes.Integer)
                    return IjsTypes.Integer;

                return IjsTypes.Double;
            }
        }

        public override INode Analyze(Stack<Function> stack)
        {
            Target = Target.Analyze(stack);
            return this;
        }
    }
}
