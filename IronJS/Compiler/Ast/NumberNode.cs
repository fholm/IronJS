using System;
using System.Text;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class NumberNode<T> : Node, INode
    {
        public T Value { get; protected set; }

        public NumberNode(T value, NodeType type, ITree node)
            : base(type, node)
        {
            Value = value;
        }

        public override Type ExprType
        {
            get
            {
                if (this.GetType() == typeof(NumberNode<int>))
                    return JsTypes.Integer;

                return JsTypes.Double;
            }
        }

        public override Et Generate2(EtGenerator etgen)
        {
            return etgen.Generate<T>(Value);
        }

        public override Et Generate(EtGenerator etgen)
        {
            return etgen.Generate<T>(Value);
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Value + ")");
        }
    }
}
