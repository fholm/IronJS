using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class InstanceOfNode : Node
    {
        public INode Target { get; protected set; }
        public INode Function { get; protected set; }

        public InstanceOfNode(INode target, INode function, ITree node)
            : base(NodeType.InstanceOf, node)
        {
            Target = target;
            Function = function;
        }

        public override Type ExprType
        {
            get
            {
                return IjsTypes.Boolean;
            }
        }

        public override INode Analyze(IjsAstAnalyzer astopt)
        {
            Target = Target.Analyze(astopt);
            Function = Target.Analyze(astopt);

            IfIdentiferUsedAs(Target, IjsTypes.Object);
            IfIdentiferUsedAs(Function, IjsTypes.Func);

            return this;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            Target.Print(writer, indent + 1);
            Function.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
