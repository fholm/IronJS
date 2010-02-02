using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class ThrowNode : Node
    {
        public INode Value { get; protected set; }

        public ThrowNode(INode value, ITree node)
            : base(NodeType.Throw, node)
        {
            Value = value;
        }

        public override INode Analyze(FuncNode astopt)
        {
            Value = Value.Analyze(astopt);
            return this;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            if (Value != null)
                Value.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
