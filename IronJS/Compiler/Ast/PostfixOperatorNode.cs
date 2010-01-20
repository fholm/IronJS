using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Scripting.Utils;
using IronJS.Runtime.Js;

using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Restrict = System.Dynamic.BindingRestrictions;
using EtParam = System.Linq.Expressions.ParameterExpression;
using IronJS.Runtime.Utils;

namespace IronJS.Compiler.Ast
{
    class PostfixOperatorNode : Node
    {
        public readonly Ast.Node Target;
        public readonly ExpressionType Op;

        public PostfixOperatorNode(Ast.Node node, ExpressionType op)
            : base(NodeType.PostfixOperator)
        {
            Target = node;
            Op = op;
        }

        public override Et Walk(EtGenerator etgen)
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
                        Target.Walk(etgen)
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
    }
}
