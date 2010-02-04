using System;
using System.Collections.Generic;
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
    using System.Text;

    public class TryNode : Node
    {
        public INode Body { get; protected set; }
        public INode Target { get; protected set; } 
        public INode Catch { get; protected set; }
        public INode Finally { get; protected set; }

        public TryNode(INode body, INode target, INode _catch, INode _finally, ITree node)
            : base(NodeType.Try, node)
        {
            Body = body;
            Catch = _catch;
            Target = target;
            Finally = _finally;
        }

        public override INode Analyze(FuncNode astopt)
        {
            Body = Body.Analyze(astopt);

            if (Target != null)
                Target = Target.Analyze(astopt);

            if (Catch != null)
                Catch = Catch.Analyze(astopt);

            if (Finally != null)
                Finally = Finally.Analyze(astopt);

            IfIdentiferUsedAs(Target, IjsTypes.Object);

            return this;
        }

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            Body.Print(writer, indent + 1);

            if (Catch != null)
                Catch.Print(writer, indent + 1);

            if (Finally != null)
            {
                string indentStr2 = new String(' ', (indent + 1) * 2);

                writer.AppendLine(indentStr2 + "(Finally");
                    Finally.Print(writer, indent + 2);
                writer.AppendLine(indentStr2 + ")");
            }

            writer.AppendLine(indentStr + ")");
        }
    }
}
