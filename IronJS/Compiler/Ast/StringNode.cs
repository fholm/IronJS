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

    public class StringNode : Node, INode
    {
        public string Value { get; protected set; }
        public char Delimiter { get; protected set; }

        public StringNode(string value, char delimiter, ITree node)
            : base(NodeType.String, node)
        {
            Value = value;
            Delimiter = delimiter;
        }

        public override Type ExprType
        {
            get
            {
                return IjsTypes.String;
            }
        }

        public override Et EtGen(FuncNode etgen)
        {
            return IjsEtGenUtils.Constant(Value);
        }

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Delimiter + Value + Delimiter + ")");
        }
    }
}
