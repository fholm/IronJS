using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class ObjectNode : Node
    {
        public Dictionary<string, INode> Properties { get; protected set; }

        public ObjectNode(Dictionary<string, INode> properties, ITree node)
            : base(NodeType.Object, node)
        {
            Properties = properties;
        }

        public override JsType ExprType
        {
            get
            {
                return JsType.Object;
            }
        }

        public override Et Generate(EtGenerator etgen)
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
                        kvp.Value.Generate(etgen)
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

            writer.AppendLine(indentStr + "(" + NodeType);

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
