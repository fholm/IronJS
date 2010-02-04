using System;
using System.Text;
using Antlr.Runtime.Tree;

using IronJS.Runtime2.Js;

namespace IronJS.Compiler.Ast
{
    public class TypeOfNode : Node
    {
        public INode Target { get; protected set; }

        public TypeOfNode(INode target, ITree node)
            : base(NodeType.TypeOf, node)
        {
            Target = target;
        }

        public override Type ExprType
        {
            get
            {
                return IjsTypes.String;
            }
        }

        public override INode Analyze(FuncNode astopt)
        {
            Target = Target.Analyze(astopt);
            return this;
        }

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            if (Target != null)
                Target.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
