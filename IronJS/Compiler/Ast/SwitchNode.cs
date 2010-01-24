using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class SwitchNode : Node, ILabelableNode
    {
        private Node Target;
        private Node Default;
        private List<Tuple<Node, Node>> Cases;
        private string Label;

        public SwitchNode(Node taret, Node _default, List<Tuple<Node, Node>> cases)
            : base(NodeType.Switch)
        {
            Target = taret;
            Default = _default;
            Cases = cases;
            Label = null;
        }

        public override Et Walk(EtGenerator etgen)
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
                    Target.Walk(etgen)
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
                                    x.V1.Walk(etgen)
                                )
                            ),
                            Et.Block(
                                x.V2.Walk(etgen),
                                Et.Assign(
                                    hasMatched,
                                    Et.Constant(true)
                                )
                            )
                        )
                    )
                ),
                Default.Walk(etgen),
                Et.Label(etgen.FunctionScope.LabelScope.Break(Label))
            );

            etgen.FunctionScope.ExitLabelScope();

            return et;
        }

        #region ILabelableNode Members

        public void SetLabel(string label)
        {
            Label = null;
        }

        #endregion
    }
}
