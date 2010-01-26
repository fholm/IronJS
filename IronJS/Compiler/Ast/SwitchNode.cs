using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class SwitchNode : Node, ILabelableNode
    {
        public Node Target { get; protected set; }
        public Node Default { get; protected set; }
        public List<Tuple<Node, Node>> Cases { get; protected set; }
        public string Label { get; protected set; }

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
                                    x.Item1.Walk(etgen)
                                )
                            ),
                            Et.Block(
                                x.Item2.Walk(etgen),
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
