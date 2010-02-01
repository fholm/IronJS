using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public enum WhileType { DoWhile, While }

    public class WhileNode : LoopNode
    {
        public INode Test { get; protected set; }
        public INode Body { get; protected set; }
        public WhileType Loop { get; protected set; }

        public WhileNode(INode test, INode body, WhileType type, ITree node)
            : base(NodeType.While, node)
        {
            Test = test;
            Body = body;
            Loop = type;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Loop);

            Test.Print(writer, indent + 1);
            Body.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
