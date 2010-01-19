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
    // 12.8
    class BreakNode : Node
    {
        public readonly string Label;

        public BreakNode(string label)
            : base(NodeType.Break)
        {
            Label = label;
        }
    
        public override Et Walk(EtGenerator etgen)
        {
            if (Label == null)
                return Et.Continue(etgen.FunctionScope.LabelScope.Break());

            return Et.Continue(etgen.FunctionScope.LabelScope.Break(Label));
        }
    }
}
