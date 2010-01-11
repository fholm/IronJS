using System;
using System.Dynamic;
using System.Reflection;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Binders
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;
    using IronJS.Runtime.Js;
    using Microsoft.Scripting.Utils;

    enum InvokeFlag { Constructor, Method, Function }

    class JsInvokeBinder : InvokeBinder
    {
        public readonly InvokeFlag CallType;
        public bool IsConstructor { get { return CallType == InvokeFlag.Constructor; } }

        public JsInvokeBinder(CallInfo callinfo, InvokeFlag callType)
            : base(callinfo)
        {
            CallType = callType;
        }

        public override Meta FallbackInvoke(Meta target, Meta[] args, Meta error)
        {
            // handles invocation of Undefined
            if (Object.ReferenceEquals(target.Value, Js.Undefined.Instance))
            {
                return EtUtils.CreateThrow(
                    target, 
                    args, 
                    Restrict.GetInstanceRestriction(
                        target.Expression, 
                        target.Value
                    ), 
                    typeof(Runtime.RuntimeError), 
                    "Can't call undefined"
                );
            }
            
            //TODO: refine this so it works properly... very ugly now
            if (target.Value is MethodInfo)
            {
                var methodInfo = (MethodInfo)target.Value;

                return new Meta(
                    Et.Call(
                        methodInfo, 
                        EtUtils.ConvertToParamTypes(
                            args,
                            methodInfo.GetParameters()
                        )
                    ),

                    RestrictUtils.BuildCallRestrictions(
                        target, 
                        args, 
                        RestrictFlag.Instance
                    )
                );
            }

            // handles call proxies that
            // are emitted from WithFrame objects
            // for function calls that really
            // are method calls
            if (target.Value is CallProxy)
            {
                var proxy = (CallProxy)target.Value;

                return new Meta(
                    Et.Dynamic(
                        new JsInvokeMemberBinder( // <- this is reason 4 with() {} is slow, 
                            proxy.Method,         // so don't frekin use it
                            new CallInfo(args.Length)
                        ),
                        typeof(object),
                        ArrayUtils.Insert(
                            Et.Field(
                                Et.Convert(
                                    target.Expression, 
                                    typeof(CallProxy)
                                ), 
                                "That"
                            ),
                            DynamicUtils.GetExpressions(args)
                        )
                    ),
                    Restrict.GetInstanceRestriction( // <- this is reason 5 with() {} is slow, 
                        target.Expression,           // so don't frekin use it
                        target.Value
                    )
                );
            }

            throw new NotImplementedException();
        }
    }
}
