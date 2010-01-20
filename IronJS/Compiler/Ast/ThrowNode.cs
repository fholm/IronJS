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
    class ThrowNode : Node
    {
        public readonly Node Target;

        public ThrowNode(Node target)
            : base(NodeType.Throw)
        {
            Target = target;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Throw(
                AstUtils.SimpleNewHelper(
                    JsRuntimeError.Ctor,
                    etgen.GenerateConvertToObject(
                        Target.Walk(etgen)
                    )
                )
            );
        }
    }
}
