using System;
using System.Text;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class AssignNode : Node, INode
    {
        public INode Target { get; protected set; }
        public INode Value { get; protected set; }

        public AssignNode(INode target, INode value, ITree node)
            : base(NodeType.Assign, node)
        {
            Target = target;
            Value = value;
        }

        public override Et Generate2(EtGenerator etgen)
        {
            return etgen.GenerateAssign2(
                Target, 
                Value.Generate2(etgen)
            );
        }

        public override Et Generate(EtGenerator etgen)
        {
            return etgen.GenerateAssign(
                Target,
                Value.Generate(etgen)
            );
        }

        public override INode Optimize(AstOptimizer astopt)
        {
            Target = Target.Optimize(astopt);
            Value = Value.Optimize(astopt);

            if (Target is IdentifierNode)
                (Target as IdentifierNode).Variable.AssignedFrom.Add(Value);

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

        #region IExpressionNode Members

        public JsType ExpressionType
        {
            get
            {
                if (Target.ExprType == Value.ExprType)
                    return Target.ExprType;

                return JsType.Dynamic;
            }
        }

        #endregion
    }
}
