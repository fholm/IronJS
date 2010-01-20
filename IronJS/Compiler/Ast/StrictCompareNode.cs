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
using IronJS.Runtime;

namespace IronJS.Compiler.Ast
{
    class StrictCompareNode : Node
    {
        public readonly Ast.Node Left;
        public readonly Ast.Node Right;
        public readonly ExpressionType Op;

        public StrictCompareNode(Ast.Node left, Ast.Node right, ExpressionType op)
            : base(NodeType.StrictCompare)
        {
            Left = left;
            Right = right;
            Op = op;
        }

        public override Expression Walk(EtGenerator etgen)
        {
            // for both
            Et expr = Et.Call(
                typeof(BuiltIns).GetMethod("StrictEquality"),
                Left.Walk(etgen),
                Right.Walk(etgen)
            );

            // specific to 11.9.5
            if (Op == ExpressionType.NotEqual)
                expr = Et.Not(Et.Convert(expr, typeof(bool)));

            return expr;
        }
    }
}
