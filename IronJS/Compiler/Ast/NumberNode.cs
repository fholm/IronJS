using System;
using System.Text;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class NumberNode : Node
    {
        public double Value { get; protected set; }

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
            return etgen.Generate<double>(Value);
        }
    }
}
