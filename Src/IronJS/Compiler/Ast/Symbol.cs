using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Tools;
using IronJS.Tools;
using IronJS.Runtime2.Js;
using System.Collections.Generic;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    using Et = Expression;

    public class Symbol : Node
    {
        public string Name { get; protected set; }

        public Symbol(string name, ITree node)
            : base(NodeType.Identifier, node)
        {
            Name = name;
        }

        public override INode Analyze(Stack<Function> stack)
        {
            return AnalyzeTools.GetVariable(stack, Name);
        }

        public override void Write(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(
                indentStr + "(" + Name + " " + TypeTools.ShortName(Type) + ")"
            );
        }
    }
}
