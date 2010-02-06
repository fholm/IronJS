using System;
using System.Text;
using Antlr.Runtime.Tree;
using Microsoft.Scripting.Ast;
using IronJS.Compiler.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    using Et = Expression;

    public class Assign : Node, INode
    {
        public INode Target { get; protected set; }
        public INode Value { get; protected set; }

        public override Type ExprType { get { return Value.ExprType; } }

        public Assign(INode target, INode value, ITree node)
            : base(NodeType.Assign, node)
        {
            Target = target;
            Value = value;
        }


        public override Et Compile(Function func)
        {
            return IjsAstTools.Assign(func, Target, Value.Compile(func));
        }

        public override INode Analyze(Function astopt)
        {
            Target = Target.Analyze(astopt);
            Value = Value.Analyze(astopt);

            IfIdentifierAssignedFrom(Target, Value);

            return this;
        }

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType + " ");

            Target.Print(writer, indent + 1);
            Value.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
