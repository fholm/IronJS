using System;
using System.Dynamic;
using System.Reflection;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using Restrict = System.Dynamic.BindingRestrictions;

namespace IronJS.Runtime.Binders
{
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
            Meta[] deferArgs;
            if (BinderUtils.NeedsToDefer(target, args, out deferArgs))
                return Defer(deferArgs);

            // handles invocation of Undefined
            if (object.ReferenceEquals(target.Value, Js.Undefined.Instance))
            {
                return new Meta(
                    InternalRuntimeError.EtNew(
                        "Can't invoke undefined"
                    ),
                    Restrict.GetInstanceRestriction(
                        target.Expression,
                        target.Value
                    )
                );
            }

            // handles invocation of null
            if (target.HasValue && target.Value == null)
            {
                return new Meta(
                    InternalRuntimeError.EtNew(
                        "Can't invoke null"
                    ),
                    Restrict.GetInstanceRestriction(
                        target.Expression,
                        target.Value
                    )
                );
            }
            
            //TODO: refine this so it works properly... very ugly now
            if (target.Value is MethodInfo)
            {
                var methodInfo = (MethodInfo)target.Value;

                return new Meta(
                    EtUtils.Box(
                        Et.Call(
                            methodInfo, 
                            EtUtils.ConvertToParamTypes(
                                ArrayUtils.RemoveFirst(args),
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

            //TODO: this should be looked over and tested properly
            if (target.Value is Delegate)
            {
                var invoke = target.LimitType.GetMethod("Invoke");

                return new Meta(
                    EtUtils.Box(
                        AstUtils.SimpleCallHelper(
                            Et.Convert(target.Expression, target.LimitType),
                            invoke,
                            DynamicUtils.GetExpressions(
                                ArrayUtils.RemoveFirst(args)
                            )
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
