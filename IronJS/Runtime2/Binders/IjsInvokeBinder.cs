using System;
using System.Dynamic;
using IronJS.Compiler.Utils;
using IronJS.Runtime2.Js;
using Et = System.Linq.Expressions.Expression;
using MetaObj = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime2.Binders
{
    public class IjsInvokeBinder : InvokeBinder
    {
        public IjsInvokeBinder(CallInfo callInfo)
            : base(callInfo)
        {

        }

        public override MetaObj FallbackInvoke(MetaObj target, MetaObj[] args, MetaObj errorSuggestion)
        {
            if (target.Value is Action<IjsFunc>)
            {
                return new MetaObj(
                    IjsEtGenUtils.Box(
                        Et.Invoke(
                           Et.Convert(target.Expression, typeof(Action<IjsFunc>)),
                           Et.Convert(args[0].Expression, typeof(IjsFunc))
                        )
                    ),
                    BindingRestrictions.GetTypeRestriction(
                        target.Expression,
                        target.LimitType
                    )
                );
            }

            if (target.Value is Func<object, object>)
            {
                return new MetaObj(
                    IjsEtGenUtils.Box(
                        Et.Invoke(
                           Et.Convert(target.Expression, typeof(Func<object, object>)),
                           Et.Convert(args[0].Expression, typeof(object))
                        )
                    ),
                    BindingRestrictions.GetTypeRestriction(
                        target.Expression,
                        target.LimitType
                    )
                );
            }

            throw new System.NotImplementedException();
        }
    }
}
