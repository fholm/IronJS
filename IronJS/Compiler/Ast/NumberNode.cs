using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;

namespace IronJS.Compiler.Ast
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;
    using IronJS.Compiler.Tools;

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
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Value + ")");
        }
    }
}
