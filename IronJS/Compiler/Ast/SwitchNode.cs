using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class SwitchNode : Node, IStatementNode
    {
        public INode Target { get; protected set; }
        public INode Default { get; protected set; }
        public List<Tuple<INode, INode>> Cases { get; protected set; }
        public string Label { get; protected set; }

        public SwitchNode(INode taret, INode _default, List<Tuple<INode, INode>> cases, ITree node)
            : base(NodeType.Switch, node)
        {
            Target = taret;
            Default = _default;
            Cases = cases;
            Label = null;
        }

        public override Et Generate(EtGenerator etgen)
        {
            etgen.FunctionScope.EnterLabelScope(Label, false);

            var tmp = Et.Variable(typeof(object), "#switch-tmp");
            var hasMatched = Et.Variable(typeof(bool), "#switch-has-matched");

            var et = Et.Block(
                new[] { tmp, hasMatched },
                Et.Assign(
                    hasMatched,
                    Et.Constant(false)
                ),
                Et.Assign(
                    tmp,
                    Target.Generate(etgen)
                ),
                Et.Block(
                    Cases.Select(x => 
                        Et.IfThen(
                            Et.MakeBinary(
                                ExpressionType.OrElse,
                                hasMatched,
                                Et.Call(
                                    typeof(Operators).GetMethod("StrictEquality"),
                                    tmp,
                                    x.Item1.Generate(etgen)
                                )
                            ),
                            Et.Block(
                                x.Item2.Generate(etgen),
                                Et.Assign(
                                    hasMatched,
                                    Et.Constant(true)
                                )
                            )
                        )
                    )
                ),
                Default.Generate(etgen),
                Et.Label(etgen.FunctionScope.LabelScope.Break(Label))
            );

            etgen.FunctionScope.ExitLabelScope();

            return et;
        }

        public override void Print(System.Text.StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            var indentStr2 = new String(' ', (indent + 1)* 2);
            var indentStr3 = new String(' ', (indent + 2) * 2);

            writer.AppendLine(indentStr + "(" + Type + "");
            Target.Print(writer, indent + 1);

            foreach (var cas in Cases)
            {
                writer.AppendLine(indentStr2 + "(Case");
                cas.Item1.Print(writer, indent + 2);
                cas.Item2.Print(writer, indent + 2);
                writer.AppendLine(indentStr2 + ")");
            }

            if (Default != null)
            {
                writer.AppendLine(indentStr2 + "(Default");
                Default.Print(writer, indent + 2);
                writer.AppendLine(indentStr2 + ")");
            }

            writer.AppendLine(indentStr + ")");
        }

        #region ILabelableNode Members

        public void SetLabel(string label)
        {
            Label = null;
        }

        #endregion
    }
}
