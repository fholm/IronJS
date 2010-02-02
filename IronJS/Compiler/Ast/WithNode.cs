using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class WithNode : Node
    {
        public INode Target { get; protected set; }
        public INode Body { get; protected set; }

        public WithNode(INode target, INode body, ITree node)
            : base(NodeType.With, node)
        {
            Target = target;
            Body = body;
        }

        public override INode Analyze(FuncNode astopt)
        {
            Target = Target.Analyze(astopt);
            Body = Body.Analyze(astopt);
            return this;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            Target.Print(writer, indent + 1);
            Body.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
