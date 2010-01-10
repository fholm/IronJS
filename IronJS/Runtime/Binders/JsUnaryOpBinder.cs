using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Linq.Expressions;
using IronJS.Compiler.Ast;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Binders
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;

    class JsUnaryOpBinder : UnaryOperationBinder
    {
        public JsUnaryOpBinder(ExpressionType op)
            : base(op)
        {

        }

        public override Meta FallbackUnaryOperation(Meta target, Meta errorSuggestion)
        {
            //TODO: insert defer

            Et expr = null;

            switch (Operation)
            {
                case ExpressionType.OnesComplement:
                    expr = Et.OnesComplement(
                        EtUtils.CastForBitOp(
                            TypeConverter.ToNumber(target)
                        )
                    );
                    break;

                case ExpressionType.Not:
                    expr = Et.Not(
                        TypeConverter.ToBoolean(target)
                    );
                    break;

                case ExpressionType.Negate:
                    expr = Et.Negate(
                        TypeConverter.ToNumber(target)
                    );
                    break;

                case ExpressionType.UnaryPlus:
                    expr = TypeConverter.ToNumber(target);
                    break;
            }

            return new Meta(
                EtUtils.Box(expr),
                Restrict.GetTypeRestriction(
                    target.Expression,
                    target.LimitType
                )
            );
        }
    }
}
