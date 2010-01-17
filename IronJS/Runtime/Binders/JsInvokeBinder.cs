using System;
using System.Dynamic;
using System.Reflection;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime.Binders
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    class JsInvokeBinder : InvokeBinder
    {
        Context _context;

        public JsInvokeBinder(CallInfo callinfo, Context context)
            : base(callinfo)
        {
            _context = context;
        }

        public override Meta FallbackInvoke(Meta target, Meta[] args, Meta error)
        {
            // handles invocation of Undefined
            if (object.ReferenceEquals(target.Value, Js.Undefined.Instance))
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

            if (target.Value is Delegate)
            {
                var invoke = target.LimitType.GetMethod("Invoke");

                return new Meta(
                    AstUtils.SimpleCallHelper(
                        Et.Convert(target.Expression, target.LimitType),
                        invoke,
                        DynamicUtils.GetExpressions(
                            ArrayUtils.RemoveFirst(args)
                        )
                    ),
                    RestrictUtils.BuildCallRestrictions(
                        target,
                        ArrayUtils.RemoveFirst(args),
                        RestrictFlag.Type
                    )
                );
            }

            throw new NotImplementedException();
        }
    }
}
