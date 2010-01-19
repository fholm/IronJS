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
    using IronJS.Runtime.Js.Utils;

    class JsUnaryOpBinder : UnaryOperationBinder
    {
        Context _context;

        public JsUnaryOpBinder(ExpressionType op, Context context)
            : base(op)
        {
            _context = context;
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
                            EtTypeConverter.ToNumber(target)
                        )
                    );
                    break;

                case ExpressionType.Not:
                    expr = Et.Not(
                        EtTypeConverter.ToBoolean(target)
                    );
                    break;

                case ExpressionType.Negate:
                    expr = Et.Negate(
                        EtTypeConverter.ToNumber(target)
                    );
                    break;

                case ExpressionType.UnaryPlus:
                    expr = EtTypeConverter.ToNumber(target);
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
