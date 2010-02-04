using System;

using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;

using Microsoft.Scripting.Ast;

namespace IronJS.Compiler.Ast
{
    public class UnaryOpNode : Node
    {
        public INode Target { get; protected set; }
        public ExpressionType Op { get; protected set; }

        public UnaryOpNode(INode target, ExpressionType op, ITree node)
            : base(NodeType.UnaryOp, node)
        {
            Target = target;
            Op = op;
        }

        public override Type ExprType
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

        public override INode Analyze(FuncNode astopt)
        {
            Target = Target.Analyze(astopt);
            return this;
        }

        public override void Print(StringBuilder writer, int indent)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Op);

            if (Target != null)
                Target.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
