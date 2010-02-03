using System;
using System.Dynamic;
using IronJS.Runtime2.Js;
using Binding = System.Dynamic.BindingRestrictions;
using MetaObj = System.Dynamic.DynamicMetaObject;
using Et = System.Linq.Expressions.Expression;
using IronJS.Compiler.Utils;

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
            if (target.Value is Action<IjsProxy>)
            {
                return new MetaObj(
                    IjsEtGenUtils.Box(
                        Et.Invoke(
                           Et.Convert(target.Expression, typeof(Action<IjsProxy>)),
                           Et.Convert(args[0].Expression, typeof(IjsProxy))
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
