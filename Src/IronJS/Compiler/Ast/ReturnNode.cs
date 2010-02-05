using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Tools;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    using Et = Expression;

    public class ReturnNode : Node
    {
        public INode Value { get; protected set; }
        public FuncNode FuncNode { get; protected set; }

        public ReturnNode(INode value, ITree node)
            : base(NodeType.Return, node)
        {
            Value = value;
        }

        public override INode Analyze(FuncNode func)
        {
            Value = Value.Analyze(func);

            FuncNode = func;
            FuncNode.Returns.Add(Value);

            return this;
        }

        public override Et Compile(FuncNode func)
        {
            return Et.Return(
                func.ReturnLabel,
				AstTools.Box(
                    Value.Compile(func)
                ),
                func.ReturnType
            );
        }

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            if (Value != null)
                Value.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
