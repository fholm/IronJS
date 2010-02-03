using System;
using System.Text;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;
using IronJS.Runtime2.Js;
using IronJS.Compiler.Utils;

namespace IronJS.Compiler.Ast
{
    public class AssignNode : Node, INode
    {
        public INode Target { get; protected set; }
        public INode Value { get; protected set; }

        public override Type ExprType { get { return Value.ExprType; } }

        public AssignNode(INode target, INode value, ITree node)
            : base(NodeType.Assign, node)
        {
            Target = target;
            Value = value;
        }


        public override Et EtGen(FuncNode func)
        {
            return IjsEtGenUtils.Assign(func, Target, Value.EtGen(func));
        }

        public override INode Analyze(FuncNode astopt)
        {
            Target = Target.Analyze(astopt);
            Value = Value.Analyze(astopt);

            IfIdentifierAssignedFrom(Target, Value);

            return this;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType + " ");

            Target.Print(writer, indent + 1);
            Value.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}
