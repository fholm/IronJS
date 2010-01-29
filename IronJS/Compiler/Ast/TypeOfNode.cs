using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

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
        
        public override Et Generate(EtGenerator etgen)
        {
            return Et.Call(
                typeof(Operators).GetMethod("TypeOf"),
                Target.Generate(etgen)
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);

            if (Target != null)
                Target.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
