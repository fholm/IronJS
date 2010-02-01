using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class MemberAccessNode : Node
    {
        public INode Target { get; protected set; }
        public string Name { get; protected set; }

        public MemberAccessNode(INode target, string member, ITree node)
            : base(NodeType.MemberAccess, node)
        {
            Target = target;
            Name = member;
        }

        public override INode Analyze(IjsAstAnalyzer astopt)
        {
            IfIdentiferUsedAs(Target, IjsTypes.Object);
            return this;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType + " " + Name);
            Target.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}
