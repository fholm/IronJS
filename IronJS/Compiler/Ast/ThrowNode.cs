using System;
using System.Text;
using IronJS.Runtime;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class ThrowNode : Node
    {
        public Node Value { get; protected set; }

        public ThrowNode(Node value)
            : base(NodeType.Throw)
        {
            Value = value;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Throw(
                AstUtils.SimpleNewHelper(
                    JsRuntimeError.Ctor,
                    etgen.GenerateConvertToObject(
                        Value.Walk(etgen)
                    )
                )
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
