using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime.Binders
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;

    using Js = IronJS.Runtime.Js;
    using Runtime = IronJS.Runtime;

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
                return Runtime.Utils.EtUtils.CreateThrow(
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
                            ArrayUtils.RemoveFirst(args),
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

            throw new NotImplementedException();
        }
    }
}
