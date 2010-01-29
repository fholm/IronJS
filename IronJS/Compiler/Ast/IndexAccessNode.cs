using System;
using System.Dynamic;
using System.Text;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class IndexAccessNode : Node
    {
        public Node Target { get; protected set; }
        public Node Index { get; protected set; }

        public IndexAccessNode(Node target, Node index)
            : base(NodeType.IndexAccess)
        {
            Target = target;
            Index = index;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Dynamic(
                etgen.Context.CreateGetIndexBinder(new CallInfo(1)),
                typeof(object),
                Target.Walk(etgen),
                Index.Walk(etgen)
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);
            Index.Print(writer, indent+1);
            Target.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}
