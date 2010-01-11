using System;
using System.Text;

namespace IronJS.Compiler.Ast
{
    using Et = System.Linq.Expressions.Expression;

    enum WhileType { Do, While }

    class WhileNode : Node
    {
        public readonly Node Test;
        public readonly Node Body;
        public readonly WhileType Loop;

        public WhileNode(Node test, Node body, WhileType type)
            : base(NodeType.While)
        {
            Test = test;
            Body = body;
            Loop = type;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Loop);

            Test.Print(writer, indent + 1);
            Body.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }

        public override Et Walk(EtGenerator etgen)
        {
            throw new NotImplementedException();
        }
    }
}
