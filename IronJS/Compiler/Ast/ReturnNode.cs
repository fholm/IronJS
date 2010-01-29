using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class ReturnNode : Node
    {
        public INode Value { get; protected set; }

        public ReturnNode(INode value, ITree node)
            : base(NodeType.Return, node)
        {
            Value = value;
        }

        public override Et Generate(EtGenerator etgen)
        {
            return Et.Return(
                etgen.FunctionScope.ReturnLabel, 
                EtUtils.Cast<object>(Value.Generate(etgen)),
                typeof(object)
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);

            if (Value != null)
                Value.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
