using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using IronJS.Compiler.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    using Et = Expression;

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

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Value + ")");
        }
    }
}
