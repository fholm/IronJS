using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    public class With : Node
    {
        public INode Target { get; protected set; }
        public INode Body { get; protected set; }

        public With(INode target, INode body, ITree node)
            : base(NodeType.With, node)
        {
            Target = target;
            Body = body;
        }

        public override void Write(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            Target.Write(writer, indent + 1);
            Body.Write(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
