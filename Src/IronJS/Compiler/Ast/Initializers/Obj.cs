using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    public class Obj : Node
    {
        public Dictionary<string, INode> Properties { get; protected set; }

        public Obj(Dictionary<string, INode> properties, ITree node)
            : base(NodeType.Object, node)
        {
            Properties = properties;
        }

        public override Type Type
        {
            get
            {
                return IjsTypes.Object;
            }
        }

        public override INode Analyze(Stack<Function> astopt)
        {
            foreach (string key in DictionaryTools.GetKeys(Properties))
                Properties[key] = Properties[key].Analyze(astopt);

            return this;
        }

        public override void Write(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);
            string indentStr2 = new String(' ', (indent + 1) * 2);
            string indentStr3 = new String(' ', (indent + 2) * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            foreach (KeyValuePair<string, INode> kvp in Properties)
            {
                writer.AppendLine(indentStr2 + "(Property ");
                writer.AppendLine(indentStr3 + "(" + kvp.Key + ")");
                kvp.Value.Write(writer, indent + 2);
                writer.AppendLine(indentStr2 + ")");
            }

            writer.AppendLine(indentStr + ")");
        }
    }
}
