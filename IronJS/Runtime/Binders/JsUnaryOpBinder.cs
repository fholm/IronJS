using System.Dynamic;

using IronJS.Runtime.Utils;

using Meta = System.Dynamic.DynamicMetaObject;
using Restrict = System.Dynamic.BindingRestrictions;
using Microsoft.Scripting.Ast;

namespace IronJS.Runtime.Binders
{
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
            if (!target.HasValue)
                return Defer(target);

            Et expr = null;

            switch (Operation)
            {
                case ExpressionType.OnesComplement:
                    expr = Et.OnesComplement(
                        EtUtils.CastForBitOp(
                            JsTypeConverter.EtToNumber(target)
                        )
                    );
                    break;

                case ExpressionType.Not:
                    expr = Et.Not(
                        JsTypeConverter.EtToBoolean(target)
                    );
                    break;

                case ExpressionType.Negate:
                    expr = Et.Negate(
                        JsTypeConverter.EtToNumber(target)
                    );
                    break;

                case ExpressionType.UnaryPlus:
                    expr = JsTypeConverter.EtToNumber(target);
                    break;
            }

            return new Meta(
                EtUtils.Box(expr),
                (target.HasValue && target.Value == null) 
                  ? Restrict.GetInstanceRestriction(
                        target.Expression,
                        null
                    )
                  : Restrict.GetTypeRestriction(
                        target.Expression,
                        target.LimitType
                    )
            );
        }
    }
}
