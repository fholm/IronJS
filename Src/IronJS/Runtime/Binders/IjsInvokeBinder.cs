using System;
using System.Dynamic;
using IronJS.Ast.Tools;
using IronJS.Runtime.Js;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Binders
{
    using Et = Expression;
    using MetaObj = DynamicMetaObject;

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
					AstTools.Box(
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
					AstTools.Box(
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
