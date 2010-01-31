using System;
using System.Dynamic;
using System.Text;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class IndexAccessNode : Node
    {
        public INode Target { get; protected set; }
        public INode Index { get; protected set; }

        public IndexAccessNode(INode target, INode index, ITree node)
            : base(NodeType.IndexAccess, node)
        {
            Target = target;
            Index = index;
        }

        public override INode Analyze(IjsAstAnalyzer astopt)
        {
            if (Target is IdentifierNode)
                (Target as IdentifierNode).VarInfo.UsedAs.Add(IjsTypes.Object);

            return this;
        }

        public override Et Generate(EtGenerator etgen)
        {
            return Et.Dynamic(
                etgen.Context.CreateGetIndexBinder(new CallInfo(1)),
                typeof(object),
                Target.Generate(etgen),
                Index.Generate(etgen)
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);
            Index.Print(writer, indent+1);
            Target.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}
