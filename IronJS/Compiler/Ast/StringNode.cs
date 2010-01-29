using System;
using System.Text;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class StringNode : Node
    {
        public string Value { get; protected set; }
        public char Delimiter { get; protected set; }

        public StringNode(string value, char delimiter, ITree node)
            : base(NodeType.String, node)
        {
            Value = value;
            Delimiter = delimiter;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "('" + Value + "')");
        }

        public override Et Generate(EtGenerator etgen)
        {
            return etgen.Generate<string>(Value);
        }
    }
}
