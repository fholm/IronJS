using System;
using System.Text;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class ReturnNode : Node
    {
        public Node Value { get; protected set; }

        public ReturnNode(Node value)
            : base(NodeType.Return)
        {
            Value = value;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Return(
                etgen.FunctionScope.ReturnLabel, 
                EtUtils.Cast<object>(Value.Walk(etgen)),
                typeof(object)
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);

            if (Value != null)
                Value.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
