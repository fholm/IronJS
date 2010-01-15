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

            // handles call proxies that
            // are emitted from WithFrame objects
            // for 'function'-calls that really
            // are method calls
            if (target.Value is PropertyProxy)
            {
                var proxy = (PropertyProxy)target.Value;

                var thatExpr = 
                    Et.Field(
                        Et.Convert(
                            target.Expression,
                            typeof(PropertyProxy)
                        ),
                        "That"
                    );

                return new Meta(
                    Et.Dynamic(
                        _context.CreateInvokeMemberBinder(
                            proxy.Name,
                            new CallInfo(args.Length)
                        ),
                        typeof(object),
                        ArrayUtils.Insert(
                            thatExpr,
                            thatExpr,
                            DynamicUtils.GetExpressions(
                                ArrayUtils.RemoveFirst(args)
                            )
                        )
                    ),
                    //TODO: more elaborate restriction
                    Restrict.GetInstanceRestriction(
                        target.Expression,
                        target.Value
                    )
                );
            }

            throw new NotImplementedException();
        }
    }
}
