using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Scripting.Utils;
using IronJS.Runtime.Js;
using IronJS.Runtime;

using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Restrict = System.Dynamic.BindingRestrictions;
using EtParam = System.Linq.Expressions.ParameterExpression;

namespace IronJS.Compiler.Ast
{
    class TypeOfNode : Node
    {
        public readonly Node Target;

        public TypeOfNode(Node target)
            : base(NodeType.TypeOf)
        {
            Target = target;
        }
        
        public override System.Linq.Expressions.Expression Walk(EtGenerator etgen)
        {
            return Et.Call(
                typeof(BuiltIns).GetMethod("TypeOf"),
                Target.Walk(etgen)
            );
        }
    }
}
