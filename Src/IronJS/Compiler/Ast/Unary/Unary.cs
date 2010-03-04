using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
using System.Collections.Generic;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    public class Unary : Node
    {
        public INode Target { get; protected set; }
        public ExpressionType Op { get; protected set; }

        public Unary(INode target, ExpressionType op, ITree node)
            : base(NodeType.UnaryOp, node)
        {
            Target = target;
            Op = op;
        }

        public override Type Type
        {
            get
            {
                if (Op == ExpressionType.Not)
                    return IjsTypes.Boolean;

                if (Op == ExpressionType.OnesComplement)
                    return IjsTypes.Integer;

                if (Op == ExpressionType.UnaryPlus)
                    return IjsTypes.Double;

                if (Op == ExpressionType.Negate)
                    return IjsTypes.Double;

                throw new AstCompilerError("Unrecognized unary operator '{0}'", Op);
            }
        }

        public override INode Analyze(Stack<Function> stack)
        {
            Target = Target.Analyze(stack);
            return this;
        }
    }
}
