using System;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class BooleanNode : Node
    {
        public bool Value { get; protected set; }

        public BooleanNode(bool value)
            : base(NodeType.Boolean)
        {
            Value = value;
        }

        public override void Print(System.Text.StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Value.ToString().ToLower() + ")");
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Constant(Value, typeof(object));
        }
    }
}
