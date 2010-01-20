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
    class IndexAccessNode : Node
    {
        public readonly Node Target;
        public readonly Node Index;

        public IndexAccessNode(Node target, Node index)
            : base(NodeType.IndexAccess)
        {
            Target = target;
            Index = index;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Dynamic(
                etgen.Context.CreateGetIndexBinder(new CallInfo(1)),
                typeof(object),
                Target.Walk(etgen),
                Index.Walk(etgen)
            );
        }
    }
}
