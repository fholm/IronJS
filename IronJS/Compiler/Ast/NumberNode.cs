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

        public override JsType ExprType
        {
            get
            {
                if (this.GetType() == typeof(NumberNode<int>))
                    return JsType.Integer;

                return JsType.Double;
            }
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Value + ")");
        }

        public override Et Generate(EtGenerator etgen)
        {
            return etgen.Generate<T>(Value);
        }
    }
}
