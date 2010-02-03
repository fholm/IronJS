using System;
using System.Text;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;
using IronJS.Compiler.Utils;
using IronJS.Runtime2.Js;

namespace IronJS.Compiler.Ast
{
    public class BooleanNode : Node
    {
        public bool Value { get; protected set; }

        public BooleanNode(bool value, ITree node)
            : base(NodeType.Boolean, node)
        {
            Value = value;
        }

        public override Type ExprType
        {
            get
            {
                return IjsTypes.Boolean;
            }
        }

        public override Et EtGen(FuncNode func)
        {
            return IjsEtGenUtils.Constant(Value);
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Value.ToString().ToLower() + ")");
        }
    }
}
