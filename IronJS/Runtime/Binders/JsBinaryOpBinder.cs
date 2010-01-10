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

    class JsBinaryOpBinder : BinaryOperationBinder
    {
        BinaryOp Op;

        public JsBinaryOpBinder(BinaryOp op)
            : base(ExpressionType.Add)
        {
            Op = op;
        }

        public override Meta FallbackBinaryOperation(Meta target, Meta arg, Meta error)
        {
            throw new NotImplementedException();
        }
    }
}
