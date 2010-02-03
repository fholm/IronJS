using System;
using System.Text;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;
using IronJS.Compiler.Utils;
using IronJS.Runtime2.Js;

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
                if (this.GetType() == typeof(NumberNode<long>))
                    return IjsTypes.Integer;

                return IjsTypes.Double;
            }
        }

        public override Et EtGen(FuncNode func)
        {
            return IjsEtGenUtils.Constant(Value);
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Value + ")");
        }
    }
}
