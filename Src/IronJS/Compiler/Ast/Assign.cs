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
    using System.Collections.Generic;

    public class Assign : Node, INode
    {
        public INode Target { get; protected set; }
        public INode Value { get; protected set; }

        public override Type Type { get { return Value.Type; } }

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

        public override INode Analyze(Stack<Function> stack)
        {
            Target = Target.Analyze(stack);
            Value = Value.Analyze(stack);

            Closed closed = Target as Closed;
            if (closed != null)
                AnalyzeTools.AddClosedType(stack, closed.Name, Value.Type);

            IfIdentifierAssignedFrom(Target, Value);

            return this;
        }

        public override void Write(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType + " ");

            Target.Write(writer, indent + 1);
            Value.Write(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
