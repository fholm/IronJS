using System;
using System.Text;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class NumberNode : Node
    {
        public double Value { get; protected set; }

        public NumberNode(double value, ITree node)
            : base(NodeType.Number, node)
        {
            Value = value;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Value + ")");
        }

        public override Et Generate(EtGenerator etgen)
        {
            return etgen.Generate<double>(Value);
        }
    }
}
