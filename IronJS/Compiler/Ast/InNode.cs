using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class InNode : Node
    {
        public INode Target { get; protected set; }
        public INode Property { get; protected set; }

        public InNode(INode target, INode property, ITree node)
            : base(NodeType.In, node)
        {
            Target = target;
            Property = property;
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
            Property = Target.Analyze(astopt);

            if (Target is IdentifierNode)
                (Target as IdentifierNode).VarInfo.UsedAs.Add(IjsTypes.Object);

            return this;
        }

        public override Et Generate(EtGenerator etgen)
        {
            return Et.Call(
                EtUtils.Cast<IObj>(Target.Generate(etgen)),
                IObjUtils.MiHas,
                Et.Call(
                    JsTypeConverter.MiToArrayIndex,
                    Property.Generate(etgen)
                )
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            Property.Print(writer, indent + 1);
            Target.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
