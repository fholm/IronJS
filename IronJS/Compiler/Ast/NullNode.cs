using System;
using System.Text;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class NullNode : Node, INode
    {
        public NullNode(ITree node)
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

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(null)");
        }
    }
}
