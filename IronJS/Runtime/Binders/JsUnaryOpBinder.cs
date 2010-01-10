using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Linq.Expressions;
using IronJS.Compiler.Ast;

namespace IronJS.Runtime.Binders
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;

    class JsUnaryOpBinder : UnaryOperationBinder
    {
        ExpressionType Op;

        public JsUnaryOpBinder(ExpressionType op)
            : base(ExpressionType.Not)
        {
            Op = op;
        }

        public override Meta FallbackUnaryOperation(Meta target, Meta errorSuggestion)
        {
            throw new NotImplementedException();
        }
    }
}
