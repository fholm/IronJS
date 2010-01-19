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

namespace IronJS.Compiler.Ast
{
    class VoidNode : Node
    {
        public readonly Ast.Node Target;

        public VoidNode(Ast.Node target)
            : base(NodeType.Void)
        {
            Target = target;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Block(
                Target.Walk(etgen),
                Undefined.Expr
            );
        }
    }
}
