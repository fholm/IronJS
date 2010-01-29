using System;
using System.Linq.Expressions;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class PostfixOperatorNode : Node
    {
        public INode Target { get; protected set; }
        public ExpressionType Op { get; protected set; }

        public PostfixOperatorNode(INode node, ExpressionType op, ITree tree)
            : base(NodeType.PostfixOperator, tree)
        {
            Target = node;
            Op = op;
        }

        public override Et Generate(EtGenerator etgen)
        {
            var tmp = Et.Parameter(typeof(double), "#tmp");

            return Et.Block(
                new[] { tmp },

                // the value we will return
                Et.Assign(
                    tmp,
                    Et.Dynamic(
                        etgen.Context.CreateConvertBinder(typeof(double)),
                        typeof(double),
                        Target.Generate(etgen)
                    )
                ),

                // calc new value
                etgen.GenerateAssign(
                    Target,
                    EtUtils.Box(
                        Et.Add(
                            tmp,
                            Et.Constant(
                                Op == ExpressionType.PostIncrementAssign
                                         ? 1.0    // 11.3.1
                                         : -1.0,  // 11.3.2
                                typeof(double)
                            )
                        )
                    )
                ),

                tmp // return the old value
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Op);
            Target.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}
