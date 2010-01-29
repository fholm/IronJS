using System;
using Et = System.Linq.Expressions.Expression;
using Antlr.Runtime.Tree;

namespace IronJS.Compiler.Ast
{
    public class BooleanNode : Node
    {
        public bool Value { get; protected set; }

        public BooleanNode(bool value, ITree node)
            : base(NodeType.Boolean, node)
        {
            Value = value;
        }

        public override void Print(System.Text.StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Value.ToString().ToLower() + ")");
        }

        public override Et Generate(EtGenerator etgen)
        {
            return etgen.Generate<bool>(Value);
        }
    }
}
