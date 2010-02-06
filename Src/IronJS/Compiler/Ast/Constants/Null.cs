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
    public class Null : Node, INode
    {
        public Null(ITree node)
            : base(NodeType.Null, node)
        {

        }

        public override Type ExprType
        {
            get
            {
                return IjsTypes.Dynamic;
            }
        }

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(null)");
        }
    }
}
