using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif


namespace IronJS.Compiler.Ast
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;
    using System.Text;

    public class Break : Node
    {
        public string Label { get; protected set; }

        public Break(string label, ITree node)
            : base(NodeType.Break, node)
        {
            Label = label;
        }

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.Append(indentStr + "(" + NodeType);

            if (Label != null)
                writer.Append(" " + Label);

            writer.AppendLine(")");
        }
    }
}
