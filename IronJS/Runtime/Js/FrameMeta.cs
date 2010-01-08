
namespace IronJS.Runtime.Js
{
    using System.Dynamic;
    using IronJS.Runtime.Utils;
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;

    class FrameMeta<T> : DynamicMetaObject
    {
        public FrameMeta(Et parameter, object value)
            : base(parameter, Restrict.Empty, value)
        {

        }

        public override Meta BindSetMember(SetMemberBinder binder, Meta value)
        {
            var keyExpr = Et.Constant(binder.Name);
            var valueExpr = EtUtils.Box(value.Expression);

            var target = Et.Call(
                Et.Convert(this.Expression, typeof(Frame<T>)),
                typeof(Frame<T>).GetMethod("Push"),
                keyExpr,
                valueExpr
            );

            var restrictions =
                Restrict.GetTypeRestriction(
                    this.Expression,
                    this.LimitType
                );

            return new Meta(target, restrictions);
        }

        public override Meta BindGetMember(GetMemberBinder binder)
        {
            var callExpr =
                Et.Call(
                    Et.Convert(this.Expression, typeof(Frame<T>)),
                    typeof(Frame<T>).GetMethod("Pull"),
                    Et.Constant(binder.Name)
                );

            var restrictions =
                Restrict.GetTypeRestriction(
                    this.Expression,
                    this.LimitType
                );

            return new Meta(callExpr, restrictions);
        }
    }

}
