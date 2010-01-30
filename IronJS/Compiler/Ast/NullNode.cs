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
                return JsTypes.Null;
            }
        }

        public override Et Generate(EtGenerator etgen)
        {
            return Et.Default(typeof(object));
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(null)");
        }
    }
}
