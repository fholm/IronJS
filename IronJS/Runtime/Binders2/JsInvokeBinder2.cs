using System;
using System.Dynamic;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using IronJS.Runtime.Utils;
using System.Reflection;

namespace IronJS.Runtime.Binders2
{
    class JsInvokeBinder2 : InvokeBinder
    {
        Context _context;

        public JsInvokeBinder2(CallInfo callInfo, Context context)
            : base(callInfo)
        {
            _context = context;
        }

        public override Meta FallbackInvoke(Meta target, Meta[] args, Meta errorSuggestion)
        {
            if (target.Value is Delegate)
            {
                var type = target.Value.GetType();
                var invoke = type.GetMethod("Invoke");

                return new Meta(
                    EtUtils.Box2(
                        Et.Call(
                            Et.Convert(target.Expression, type),
                            invoke
                        )
                    ),
                    RestrictUtils.BuildCallRestrictions(
                        target,
                        args,
                        RestrictFlag.Type
                    )
                );
            }


            //TODO: refine this so it works properly... very ugly now
            var methodInfo = target.Value as MethodInfo;
            if (methodInfo != null)
            {
                return new Meta(
                    EtUtils.Box(
                        Et.Call(
                            methodInfo,
                            EtUtils.ConvertToParamTypes(
                                args,
                                methodInfo.GetParameters()
                            )
                        )
                    ),
                    RestrictUtils.BuildCallRestrictions(
                        target,
                        args,
                        RestrictFlag.Instance
                    )
                );
            }

            throw new NotImplementedException();
        }
    }
}
