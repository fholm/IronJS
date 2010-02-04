using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using IronJS.Tools;

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

        public override Type ExprType
        {
            get
            {
                return IjsTypes.Object;
            }
        }

        public override INode Analyze(FuncNode astopt)
        {
            foreach (string key in DictionaryTools.GetKeys(Properties))
                Properties[key] = Properties[key].Analyze(astopt);

            return this;
        }

        public override void Print(StringBuilder writer, int indent)
        {
            var indentStr = new String(' ', indent * 2);
            var indentStr2 = new String(' ', (indent + 1) * 2);
            var indentStr3 = new String(' ', (indent + 2) * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            foreach (KeyValuePair<string, INode> kvp in Properties)
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
