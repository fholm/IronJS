using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class ObjectNode : Node
    {
        public Dictionary<string, Node> Properties { get; protected set; }

        public ObjectNode(Dictionary<string, Node> properties)
            : base(NodeType.Object)
        {
            Properties = properties;
        }

        public override Et Walk(EtGenerator etgen)
        {
            var tmp = Et.Parameter(typeof(IObj), "#tmp");
            var exprs = new List<Et>();

            foreach (var kvp in Properties)
            {
                exprs.Add(
                    Et.Call(
                        IObjUtils.MiSetObj,
                        tmp,
                        Et.Constant(kvp.Key, typeof(object)),
                        kvp.Value.Walk(etgen)
                    )
                );
            }

            return Et.Block(
                new[] { tmp },
                Et.Assign(
                    tmp,
                    Et.Call(
                        Et.Constant(etgen.Context),
                        Context.MiCreateObject
                    )
                ),
                etgen.BlockIfNotEmpty(exprs),
                tmp
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            var indentStr2 = new String(' ', (indent + 1) * 2);
            var indentStr3 = new String(' ', (indent + 2) * 2);

            writer.AppendLine(indentStr + "(" + Type);

            foreach (var kvp in Properties)
            {
                writer.AppendLine(indentStr2 + "(Property ");
                writer.AppendLine(indentStr3 + "(" + kvp.Key + ")");
                kvp.Value.Print(writer, indent + 2);
                writer.AppendLine(indentStr2 + ")");
            }

            writer.AppendLine(indentStr + ")");
        }
    }
}
