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

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Value.ToString().ToLower() + ")");
        }
    }
}
