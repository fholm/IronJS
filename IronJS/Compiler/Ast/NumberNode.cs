using System;
using System.Text;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class NumberNode : Node
    {
        public readonly double Value;

        public NumberNode(double value)
            : base(NodeType.Number)
        {
            Value = value;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Type + " " + Value + ")");
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Constant(Value, typeof(object));
        }
    }
}
